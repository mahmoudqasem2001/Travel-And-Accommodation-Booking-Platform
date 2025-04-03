using System.Linq.Expressions;
using AccommodationBookingPlatform.Application.Rooms.GetForManagement;
using AccommodationBookingPlatform.Domain.Entities;
using AccommodationBookingPlatform.Domain.Enums;
using AccommodationBookingPlatform.Domain.Models;
using AutoMapper;
using Domain.Exceptions;
using Domain.Interfaces.Persistence.Repositories;
using Domain.Models;
using Moq;
using Xunit;


public class GetRoomsHandlerTests
{
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IRoomClassRepository> _roomClassRepositoryMock;
    private readonly Mock<IRoomRepository> _roomRepositoryMock;
    private readonly GetRoomsHandler _handler;

    public GetRoomsHandlerTests()
    {
        _mapperMock = new Mock<IMapper>();
        _roomClassRepositoryMock = new Mock<IRoomClassRepository>();
        _roomRepositoryMock = new Mock<IRoomRepository>();

        _handler = new GetRoomsHandler(
            _roomRepositoryMock.Object,
            _mapperMock.Object,
            _roomClassRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenRoomClassDoesNotExist()
    {
        // Arrange
        var query = new GetRoomsForManagementQuery
        {
            RoomClassId = Guid.NewGuid(),
            PageNumber = 1,
            PageSize = 10,
            SearchTerm = "101",
            SortOrder = SortOrder.Ascending,
            SortColumn = "Number"
        };

        _roomClassRepositoryMock
            .Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<RoomClass, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(query, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyPaginatedList_WhenNoRoomsAreFound()
    {
        // Arrange
        var query = new GetRoomsForManagementQuery
        {
            RoomClassId = Guid.NewGuid(),
            PageNumber = 1,
            PageSize = 10
        };

        _roomClassRepositoryMock
            .Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<RoomClass, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var emptyList = new PaginatedList<RoomForManagement>(new List<RoomForManagement>(), new PaginationMetadata(1, 10, 0));

        _roomRepositoryMock
            .Setup(repo => repo.GetForManagementAsync(It.IsAny<Query<Room>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(emptyList);

        _mapperMock
            .Setup(mapper => mapper.Map<PaginatedList<RoomForManagementResponse>>(emptyList))
            .Returns(new PaginatedList<RoomForManagementResponse>(new List<RoomForManagementResponse>(), new PaginationMetadata(1, 10, 0)));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Empty(result.Items);
    }

    [Fact]
    public async Task Handle_ShouldReturnPaginatedListOfRooms_WhenRoomsAreAvailable()
    {
        // Arrange
        var query = new GetRoomsForManagementQuery
        {
            RoomClassId = Guid.NewGuid(),
            PageNumber = 1,
            PageSize = 2
        };

        var roomsForManagement = new List<RoomForManagement>
{
    new RoomForManagement { Id = Guid.NewGuid(), RoomClassId = Guid.NewGuid(), Number = "101", IsAvailable = true, CreatedAtUtc = DateTime.UtcNow },
    new RoomForManagement { Id = Guid.NewGuid(), RoomClassId = Guid.NewGuid(), Number = "102", IsAvailable = false, CreatedAtUtc = DateTime.UtcNow }
};
        var paginationMetadata = new PaginationMetadata(1, 2, roomsForManagement.Count);
        var paginatedRooms = new PaginatedList<RoomForManagement>(roomsForManagement, paginationMetadata);

        _roomClassRepositoryMock
            .Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<RoomClass, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _roomRepositoryMock
            .Setup(repo => repo.GetForManagementAsync(It.IsAny<Query<Room>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(paginatedRooms);

        _mapperMock
            .Setup(mapper => mapper.Map<PaginatedList<RoomForManagementResponse>>(paginatedRooms))
            .Returns(new PaginatedList<RoomForManagementResponse>(
                roomsForManagement.Select(r => new RoomForManagementResponse { Id = r.Id, Number = r.Number }).ToList(),
                paginationMetadata));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(2, result.Items.Count());
        Assert.Equal("101", result.Items.ToList()[0].ToString());
        Assert.Equal("102", result.Items.ToList()[1].ToString());
    }
}

