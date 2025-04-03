using Xunit;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AccommodationBookingPlatform.Domain.Entities;
using AccommodationBookingPlatform.Domain.Interfaces.Persistence.Repositories;
using Application.Users.Register;
using Domain.Exceptions;
using Domain.Interfaces.Persistence;
using Domain.Messages;
using AccommodationBookingPlatform.Application.Users.Register;

public class RegisterCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IRoleRepository> _roleRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly RegisterCommandHandler _handler;

    public RegisterCommandHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _roleRepositoryMock = new Mock<IRoleRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();

        _handler = new RegisterCommandHandler(
            _userRepositoryMock.Object,
            _mapperMock.Object,
            _roleRepositoryMock.Object,
            _unitOfWorkMock.Object
        );
    }

    [Fact]
    public async Task Handle_ValidRequest_CreatesUserAndCommits()
    {
        // Arrange
        var request = new RegisterCommand { Email = "test@example.com", Role = "User" };
        var role = new Role { Name = "User" };
        var user = new User { Email = request.Email };

        _roleRepositoryMock.Setup(repo => repo.GetByNameAsync(request.Role, It.IsAny<CancellationToken>()))
            .ReturnsAsync(role);

        _userRepositoryMock.Setup(repo => repo.ExistsByEmailAsync(request.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _mapperMock.Setup(mapper => mapper.Map<User>(request))
            .Returns(user);

        // Act
        await _handler.Handle(request, CancellationToken.None);

        // Assert
        _userRepositoryMock.Verify(repo => repo.CreateAsync(user, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_InvalidRole_ThrowsInvalidRoleException()
    {
        // Arrange
        var request = new RegisterCommand { Email = "test@example.com", Role = "NonExistentRole" };

        _roleRepositoryMock.Setup(repo => repo.GetByNameAsync(request.Role, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Role)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidRoleException>(() =>
            _handler.Handle(request, CancellationToken.None));

        Assert.Equal(UserMessages.InvalidRole, exception.Message);
    }

    [Fact]
    public async Task Handle_EmailAlreadyExists_ThrowsDuplicateEmailUserException()
    {
        // Arrange
        var request = new RegisterCommand { Email = "test@example.com", Role = "User" };

        _roleRepositoryMock.Setup(repo => repo.GetByNameAsync(request.Role, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Role { Name = "User" });

        _userRepositoryMock.Setup(repo => repo.ExistsByEmailAsync(request.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<DuplicateEmailUserException>(() =>
            _handler.Handle(request, CancellationToken.None));

        Assert.Equal(UserMessages.WithEmailExists, exception.Message);
    }
}
