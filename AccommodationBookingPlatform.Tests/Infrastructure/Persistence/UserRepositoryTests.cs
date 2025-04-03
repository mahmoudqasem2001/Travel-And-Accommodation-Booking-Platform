using AccommodationBookingPlatform.Domain.Entities;
using AccommodationBookingPlatform.Infrastructure.Presistence.DbContexts;
using AccommodationBookingPlatform.Infrastructure.Presistence.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

public class UserRepositoryTests
{
    private readonly HotelBookingDbContext _context;
    private readonly Mock<IPasswordHasher<User>> _passwordHasherMock;
    private readonly UserRepository _userRepository;

    public UserRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<HotelBookingDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")  
            .Options;

        _context = new HotelBookingDbContext(options);
        _passwordHasherMock = new Mock<IPasswordHasher<User>>();
        _userRepository = new UserRepository(_context, _passwordHasherMock.Object);
    }

    [Fact]
    public async Task AuthenticateAsync_ValidCredentials_ReturnsUser()
    {
        // Arrange
        var user = new User { Id = Guid.NewGuid(), Email = "test@example.com", Password = "hashedPassword" };
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        _passwordHasherMock
            .Setup(ph => ph.VerifyHashedPassword(user, user.Password, "validPassword"))
            .Returns(PasswordVerificationResult.Success);

        // Act
        var result = await _userRepository.AuthenticateAsync("test@example.com", "validPassword");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Email, result.Email);
    }

    [Fact]
    public async Task AuthenticateAsync_InvalidCredentials_ReturnsNull()
    {
        // Arrange
        var user = new User { Id = Guid.NewGuid(), Email = "test@example.com", Password = "hashedPassword" };
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        _passwordHasherMock
            .Setup(ph => ph.VerifyHashedPassword(user, user.Password, "wrongPassword"))
            .Returns(PasswordVerificationResult.Failed);

        // Act
        var result = await _userRepository.AuthenticateAsync("test@example.com", "wrongPassword");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_ValidUser_HashesPasswordAndAddsToDatabase()
    {
        // Arrange
        var user = new User { Id = Guid.NewGuid(), Email = "test@example.com", Password = "plainPassword" };
        _passwordHasherMock.Setup(ph => ph.HashPassword(user, "plainPassword")).Returns("hashedPassword");

        // Act
        await _userRepository.CreateAsync(user);
        await _context.SaveChangesAsync();

        // Assert
        var storedUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == "test@example.com");
        Assert.NotNull(storedUser);
        Assert.Equal("hashedPassword", storedUser.Password);
    }

    [Fact]
    public async Task ExistsByEmailAsync_EmailExists_ReturnsTrue()
    {
        // Arrange
        var user = new User { Id = Guid.NewGuid(), Email = "exists@example.com" };
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _userRepository.ExistsByEmailAsync("exists@example.com");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ExistsByEmailAsync_EmailDoesNotExist_ReturnsFalse()
    {
        // Act
        var result = await _userRepository.ExistsByEmailAsync("notfound@example.com");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ExistsByIdAsync_IdExists_ReturnsTrue()
    {
        // Arrange
        var user = new User { Id = Guid.NewGuid(), Email = "test@example.com" };
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _userRepository.ExistsByIdAsync(user.Id);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task GetByIdAsync_ValidId_ReturnsUser()
    {
        // Arrange
        var user = new User { Id = Guid.NewGuid(), Email = "test@example.com" };
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _userRepository.GetByIdAsync(user.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Email, result.Email);
    }
}
