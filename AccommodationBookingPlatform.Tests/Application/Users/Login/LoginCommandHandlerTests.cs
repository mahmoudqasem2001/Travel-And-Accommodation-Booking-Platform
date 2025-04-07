

using AccommodationBookingPlatform.Application.Users.Login;
using AccommodationBookingPlatform.Domain.Entities;
using AccommodationBookingPlatform.Domain.Interfaces.Auth;
using AccommodationBookingPlatform.Domain.Interfaces.Persistence.Repositories;
using AccommodationBookingPlatform.Infrastructure.Auth.Jwt;
using Application.Users.Login;
using AutoMapper;
using Domain.Exceptions;
using Domain.Messages;
using Domain.Models;
using Moq;
using Xunit;

namespace AccommodationBookingPlatform.Tests.Application.Users.Login
{

    public class LoginCommandHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IJwtTokenGenerator> _jwtTokenGeneratorMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly LoginCommandHandler _handler;

        public LoginCommandHandlerTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _jwtTokenGeneratorMock = new Mock<IJwtTokenGenerator>();
            _mapperMock = new Mock<IMapper>();

            _handler = new LoginCommandHandler(
                _userRepositoryMock.Object,
                _jwtTokenGeneratorMock.Object,
                _mapperMock.Object
            );
        }

        [Fact]
        public async Task Handle_ValidCredentials_ReturnsLoginResponse()
        {
            // Arrange
            var request = new LoginCommand { Email = "test@example.com", Password = "password123" };
            var user = new User { Email = "test@example.com" };
            var jwtToken = new JwtToken ("valid-token");
            var expectedResponse = new LoginResponse { Token = "valid-token" };

            _userRepositoryMock.Setup(repo => repo.AuthenticateAsync(request.Email, request.Password, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);
            _jwtTokenGeneratorMock.Setup(gen => gen.Generate(user)).Returns(jwtToken);
            _mapperMock.Setup(map => map.Map<LoginResponse>(jwtToken)).Returns(expectedResponse);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedResponse.Token, result.Token);
        }

        [Theory]
        [InlineData("wrong@example.com", "password123")]
        [InlineData("test@example.com", "wrongpassword")]
        public async Task Handle_InvalidCredentials_ThrowsException(string email, string password)
        {
            // Arrange
            var request = new LoginCommand { Email = email, Password = password };

            _userRepositoryMock.Setup(repo => repo.AuthenticateAsync(email, password, It.IsAny<CancellationToken>()))
                .ReturnsAsync(null as User);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<CredentialsNotValidException>(async () =>
                await _handler.Handle(request, CancellationToken.None));

            Assert.Equal(UserMessages.CredentialsNotValid, exception.Message);
        }

        [Fact]
        public async Task Handle_UserRepositoryThrowsException_PropagatesException()
        {
            // Arrange
            var request = new LoginCommand { Email = "test@example.com", Password = "password123" };

            _userRepositoryMock.Setup(repo => repo.AuthenticateAsync(request.Email, request.Password, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new System.Exception("Database error"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<System.Exception>(async () =>
                await _handler.Handle(request, CancellationToken.None));

            Assert.Equal("Database error", exception.Message);
        }
    }
}
