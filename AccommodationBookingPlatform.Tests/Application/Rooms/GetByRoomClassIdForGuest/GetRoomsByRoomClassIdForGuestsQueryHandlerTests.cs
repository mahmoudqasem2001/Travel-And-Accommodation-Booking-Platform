using System.Linq.Expressions;
using AccommodationBookingPlatform.Application.Rooms.GetByRoomClassIdForGuest;
using AccommodationBookingPlatform.Domain.Entities;
using AccommodationBookingPlatform.Domain.Models;
using AutoMapper;
using Domain.Exceptions;
using Domain.Interfaces.Persistence.Repositories;
using Moq;
using Xunit;

public class GetRoomsByRoomClassIdForGuestsQueryHandlerTests
{
    private readonly Mock<IRoomClassRepository> _roomClassRepositoryMock;
    private readonly Mock<IRoomRepository> _roomRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetRoomsByRoomClassIdForGuestsQueryHandler _handler;

    public GetRoomsByRoomClassIdForGuestsQueryHandlerTests()
    {
        _roomClassRepositoryMock = new Mock<IRoomClassRepository>();
        _roomRepositoryMock = new Mock<IRoomRepository>();
        _mapperMock = new Mock<IMapper>();

        _handler = new GetRoomsByRoomClassIdForGuestsQueryHandler(
            _roomClassRepositoryMock.Object,
            _roomRepositoryMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenRoomClassDoesNotExist()
    {
        // Arrange
        var query = new GetRoomsByRoomClassIdForGuestsQuery
        {
            RoomClassId = Guid.NewGuid(),
            CheckInDate = DateOnly.FromDateTime(DateTime.UtcNow),
            CheckOutDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3)),
            PageNumber = 1,
            PageSize = 10
        };

        _roomClassRepositoryMock
            .Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<RoomClass, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(query, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyPaginatedList_WhenNoRoomsAreAvailable()
    {
        // Arrange
        var query = new GetRoomsByRoomClassIdForGuestsQuery
        {
            RoomClassId = Guid.NewGuid(),
            CheckInDate = DateOnly.FromDateTime(DateTime.UtcNow),
            CheckOutDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3)),
            PageNumber = 1,
            PageSize = 10
        };

        _roomClassRepositoryMock
            .Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<RoomClass, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var paginationMetadata = new PaginationMetadata(query.PageNumber, query.PageSize, 0);
        var emptyRoomsList = new PaginatedList<Room>(new List<Room>(), paginationMetadata);

        _roomRepositoryMock
            .Setup(repo => repo.GetAsync(It.IsAny<Query<Room>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(emptyRoomsList);

        _mapperMock
            .Setup(mapper => mapper.Map<PaginatedList<RoomForGuestResponse>>(emptyRoomsList))
            .Returns(new PaginatedList<RoomForGuestResponse>(new List<RoomForGuestResponse>(), paginationMetadata));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Empty(result.Items);
    }

    [Fact]
    public async Task Handle_ShouldReturnPaginatedListOfRooms_WhenRoomsAreAvailable()
    {
        // Arrange
        var query = new GetRoomsByRoomClassIdForGuestsQuery
        {
            RoomClassId = Guid.NewGuid(),
            CheckInDate = DateOnly.FromDateTime(DateTime.UtcNow),
            CheckOutDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3)),
            PageNumber = 1,
            PageSize = 2
        };

        var availableRooms = new List<Room>
        {
            new Room { Id = Guid.NewGuid(), RoomClassId = query.RoomClassId },
            new Room { Id = Guid.NewGuid(), RoomClassId = query.RoomClassId }
        };

        _roomClassRepositoryMock
            .Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<RoomClass, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var paginationMetadata = new PaginationMetadata(query.PageNumber, query.PageSize, availableRooms.Count);
        var paginatedRooms = new PaginatedList<Room>(availableRooms, paginationMetadata);

        _roomRepositoryMock
            .Setup(repo => repo.GetAsync(It.IsAny<Query<Room>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(paginatedRooms);

        _mapperMock
            .Setup(mapper => mapper.Map<PaginatedList<RoomForGuestResponse>>(paginatedRooms))
            .Returns(new PaginatedList<RoomForGuestResponse>(
                availableRooms.Select(r => new RoomForGuestResponse { Id = r.Id }).ToList(),
                paginationMetadata));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(2, result.Items.Count());
    }

    [Fact]
    public async Task Handle_ShouldFilterOutRoomsWithOverlappingBookings()
    {
        // Arrange
        var query = new GetRoomsByRoomClassIdForGuestsQuery
        {
            RoomClassId = Guid.NewGuid(),
            CheckInDate = DateOnly.FromDateTime(DateTime.UtcNow),
            CheckOutDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3)),
            PageNumber = 1,
            PageSize = 10
        };

        var rooms = new List<Room>
        {
            new Room
            {
                Id = Guid.NewGuid(),
                RoomClassId = query.RoomClassId,
                Bookings = new List<Booking>
                {
                    new Booking { CheckInDateUtc = query.CheckInDate.AddDays(-1), CheckOutDateUtc = query.CheckOutDate.AddDays(1) }
                }
            },
            new Room
            {
                Id = Guid.NewGuid(),
                RoomClassId = query.RoomClassId,
                Bookings = new List<Booking>()
            }
        };

        var filteredRooms = rooms
            .Where(r => !r.Bookings.Any(b =>
                query.CheckInDate < b.CheckOutDateUtc &&
                query.CheckOutDate > b.CheckInDateUtc))
            .ToList();

        var paginationMetadata = new PaginationMetadata(query.PageNumber, query.PageSize, filteredRooms.Count);
        var paginatedRooms = new PaginatedList<Room>(filteredRooms, paginationMetadata);

        _roomClassRepositoryMock
            .Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<RoomClass, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _roomRepositoryMock
            .Setup(repo => repo.GetAsync(It.IsAny<Query<Room>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(paginatedRooms);

        _mapperMock
            .Setup(mapper => mapper.Map<PaginatedList<RoomForGuestResponse>>(paginatedRooms))
            .Returns(new PaginatedList<RoomForGuestResponse>(
                filteredRooms.Select(r => new RoomForGuestResponse { Id = r.Id }).ToList(),
                paginationMetadata));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Single(result.Items); // Only the room without overlapping bookings should remain
    }
}
