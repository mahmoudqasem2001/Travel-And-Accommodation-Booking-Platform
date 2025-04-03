using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AccommodationBookingPlatform.Application.Rooms.Delete;
using AccommodationBookingPlatform.Domain.Entities;
using AutoMapper;
using Domain.Exceptions;
using Domain.Interfaces.Persistence;
using Domain.Interfaces.Persistence.Repositories;
using Domain.Messages;
using Moq;
using Xunit;

public class DeleteRoomCommandHandlerTests
{
    private readonly Mock<IRoomRepository> _roomRepositoryMock;
    private readonly Mock<IBookingRepository> _bookingRepositoryMock;
    private readonly Mock<IRoomClassRepository> _roomClassRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly DeleteRoomCommandHandler _handler;

    public DeleteRoomCommandHandlerTests()
    {
        _roomRepositoryMock = new Mock<IRoomRepository>();
        _bookingRepositoryMock = new Mock<IBookingRepository>();
        _roomClassRepositoryMock = new Mock<IRoomClassRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _handler = new DeleteRoomCommandHandler(
            _roomRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _roomClassRepositoryMock.Object,
            _bookingRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenRoomClassDoesNotExist()
    {
        // Arrange
        var command = new DeleteRoomCommand { RoomClassId = Guid.NewGuid(), RoomId = Guid.NewGuid() };

        _roomClassRepositoryMock
            .Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<RoomClass, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenRoomDoesNotExist()
    {
        // Arrange
        var command = new DeleteRoomCommand { RoomClassId = Guid.NewGuid(), RoomId = Guid.NewGuid() };

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
    public async Task Handle_ShouldThrowResourceHasDependentsException_WhenRoomHasActiveBookings()
    {
        // Arrange
        var command = new DeleteRoomCommand { RoomClassId = Guid.NewGuid(), RoomId = Guid.NewGuid() };

        _roomClassRepositoryMock
            .Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<RoomClass, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _roomRepositoryMock
            .Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<Room, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _bookingRepositoryMock
            .Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<Booking, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<ResourceHasDependentsException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldDeleteRoomAndCommitChanges_WhenRoomIsValid()
    {
        // Arrange
        var command = new DeleteRoomCommand { RoomClassId = Guid.NewGuid(), RoomId = Guid.NewGuid() };

        _roomClassRepositoryMock
            .Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<RoomClass, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _roomRepositoryMock
            .Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<Room, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _bookingRepositoryMock
            .Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<Booking, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _roomRepositoryMock
            .Setup(repo => repo.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _roomRepositoryMock.Verify(repo => repo.DeleteAsync(command.RoomId, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
