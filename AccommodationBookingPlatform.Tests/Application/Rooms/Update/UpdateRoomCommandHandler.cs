using Moq;
using Xunit;
using AutoMapper;
using AccommodationBookingPlatform.Application.Rooms.Update;
using Domain.Exceptions;
using Domain.Interfaces.Persistence.Repositories;
using AccommodationBookingPlatform.Domain.Entities;
using Domain.Interfaces.Persistence;
using System.Linq.Expressions;

public class UpdateRoomCommandHandlerTests
{
    private readonly Mock<IRoomClassRepository> _roomClassRepositoryMock;
    private readonly Mock<IRoomRepository> _roomRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly UpdateRoomCommandHandler _handler;

    public UpdateRoomCommandHandlerTests()
    {
        _roomClassRepositoryMock = new Mock<IRoomClassRepository>();
        _roomRepositoryMock = new Mock<IRoomRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();

        _handler = new UpdateRoomCommandHandler(
            _roomRepositoryMock.Object,
            _roomClassRepositoryMock.Object,
            _mapperMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenRoomClassDoesNotExist()
    {
        // Arrange
        var command = new UpdateRoomCommand
        {
            RoomClassId = Guid.NewGuid(),
            RoomId = Guid.NewGuid(),
            Number = "101"
        };

        _roomClassRepositoryMock
            .Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<RoomClass, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldThrowDuplicateRoomClassException_WhenRoomNumberAlreadyExists()
    {
        // Arrange
        var command = new UpdateRoomCommand
        {
            RoomClassId = Guid.NewGuid(),
            RoomId = Guid.NewGuid(),
            Number = "101"
        };

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
    public async Task Handle_ShouldThrowNotFoundException_WhenRoomDoesNotExist()
    {
        // Arrange
        var command = new UpdateRoomCommand
        {
            RoomClassId = Guid.NewGuid(),
            RoomId = Guid.NewGuid(),
            Number = "101"
        };

        _roomClassRepositoryMock
            .Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<RoomClass, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _roomRepositoryMock
            .Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<Room, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldSuccessfullyUpdateRoom_WhenRoomIsValid()
    {
        // Arrange
        var command = new UpdateRoomCommand
        {
            RoomClassId = Guid.NewGuid(),
            RoomId = Guid.NewGuid(),
            Number = "101"
        };

        var roomEntity = new Room
        {
            Id = command.RoomId,
            RoomClassId = command.RoomClassId,
            Number = "100"
        };

        _roomClassRepositoryMock
            .Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<RoomClass, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _roomRepositoryMock
            .Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<Room, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _roomRepositoryMock
            .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(roomEntity);

        _mapperMock
            .Setup(mapper => mapper.Map(It.IsAny<UpdateRoomCommand>(), It.IsAny<Room>()))
            .Callback<UpdateRoomCommand, Room>((cmd, room) =>
            {
                room.Number = cmd.Number;
            });

        _roomRepositoryMock
            .Setup(repo => repo.UpdateAsync(It.IsAny<Room>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _unitOfWorkMock
      .Setup(unitOfWork => unitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()))
      .ReturnsAsync(1);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _roomRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Room>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(unitOfWork => unitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
