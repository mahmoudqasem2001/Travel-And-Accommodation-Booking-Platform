using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AccommodationBookingPlatform.Application.Rooms.Create;
using AccommodationBookingPlatform.Domain.Entities;
using AutoMapper;
using Domain.Exceptions;
using Domain.Interfaces.Persistence;
using Domain.Interfaces.Persistence.Repositories;
using Domain.Messages;
using Moq;
using Xunit;

public class CreateRoomHandlerTests
{
    private readonly Mock<IRoomRepository> _roomRepositoryMock;
    private readonly Mock<IRoomClassRepository> _roomClassRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly CreateRoomHandler _handler;

    public CreateRoomHandlerTests()
    {
        _roomRepositoryMock = new Mock<IRoomRepository>();
        _roomClassRepositoryMock = new Mock<IRoomClassRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _handler = new CreateRoomHandler(
            _roomRepositoryMock.Object,
            _roomClassRepositoryMock.Object,
            _mapperMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenRoomClassDoesNotExist()
    {
        // Arrange
        var command = new CreateRoomCommand { RoomClassId = Guid.NewGuid(), Number = "101" };
        _roomClassRepositoryMock
            .Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<RoomClass, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldThrowDuplicateRoomClassException_WhenRoomWithSameNumberExists()
    {
        // Arrange
        var command = new CreateRoomCommand { RoomClassId = Guid.NewGuid(), Number = "101" };
        _roomClassRepositoryMock
            .Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<RoomClass, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _roomRepositoryMock
            .Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<Room, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<DuplicateRoomClassException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldCreateRoomAndReturnId_WhenRoomIsValid()
    {
        // Arrange
        var command = new CreateRoomCommand { RoomClassId = Guid.NewGuid(), Number = "101" };
        var room = new Room { Id = Guid.NewGuid(), RoomClassId = command.RoomClassId, Number = command.Number };

        _roomClassRepositoryMock
            .Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<RoomClass, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _roomRepositoryMock
            .Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<Room, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _mapperMock
            .Setup(mapper => mapper.Map<Room>(command))
            .Returns(room);
        _roomRepositoryMock
            .Setup(repo => repo.CreateAsync(It.IsAny<Room>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(room);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(room.Id, result);
        _roomRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<Room>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowException_WhenSaveChangesFails()
    {
        // Arrange
        var command = new CreateRoomCommand { RoomClassId = Guid.NewGuid(), Number = "101" };
        var room = new Room { Id = Guid.NewGuid(), RoomClassId = command.RoomClassId, Number = command.Number };

        _roomClassRepositoryMock
            .Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<RoomClass, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _roomRepositoryMock
            .Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<Room, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _mapperMock
            .Setup(mapper => mapper.Map<Room>(command))
            .Returns(room);
        _roomRepositoryMock
            .Setup(repo => repo.CreateAsync(It.IsAny<Room>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(room);
        _unitOfWorkMock
            .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Handle_ShouldThrowArgumentException_WhenRoomNumberIsInvalid(int invalidNumber)
    {
        // Arrange
        var command = new CreateRoomCommand { RoomClassId = Guid.NewGuid(), Number = invalidNumber.ToString() };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _handler.Handle(command, CancellationToken.None));
    }
}
