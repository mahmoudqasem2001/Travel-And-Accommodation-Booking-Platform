using AccommodationBookingPlatform.Domain.Entities;
using AccommodationBookingPlatform.Infrastructure.Presistence.Repositories;
using AccommodationBookingPlatform.Tests.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace AccommodationBookingPlatform.Tests.Infrastructure.Persistence;

public class UserRepositoryTests
{
    private readonly InMemoryDbContextFactory _dbContextFactory = new();
    private readonly Mock<IPasswordHasher<User>> _passwordHasherMock = new();

    [Fact]
    public async Task AuthenticateAsync_ValidCredentials_ReturnsUser()
    {
        using var context = _dbContextFactory.CreateContext();
        var repository = new UserRepository(context, _passwordHasherMock.Object);
        var user = EntityFactory.CreateUser(email: "test@example.com");
        user.Password = "hashedPassword";
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        _passwordHasherMock
            .Setup(ph => ph.VerifyHashedPassword(user, "hashedPassword", "validPassword"))
            .Returns(PasswordVerificationResult.Success);

        var result = await repository.AuthenticateAsync("test@example.com", "validPassword");

        Assert.NotNull(result);
        Assert.Equal(user.Email, result!.Email);
    }

    [Fact]
    public async Task AuthenticateAsync_InvalidCredentials_ReturnsNull()
    {
        using var context = _dbContextFactory.CreateContext();
        var repository = new UserRepository(context, _passwordHasherMock.Object);
        var user = EntityFactory.CreateUser(email: "test@example.com");
        user.Password = "hashedPassword";
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        _passwordHasherMock
            .Setup(ph => ph.VerifyHashedPassword(user, user.Password, "wrongPassword"))
            .Returns(PasswordVerificationResult.Failed);

        var result = await repository.AuthenticateAsync("test@example.com", "wrongPassword");

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_ValidUser_HashesPasswordAndAddsToDatabase()
    {
        using var context = _dbContextFactory.CreateContext();
        var repository = new UserRepository(context, _passwordHasherMock.Object);
        var user = EntityFactory.CreateUser(email: "test@example.com");
        user.Password = "plainPassword";
        _passwordHasherMock.Setup(ph => ph.HashPassword(user, "plainPassword")).Returns("hashedPassword");

        await repository.CreateAsync(user);
        await context.SaveChangesAsync();

        var storedUser = await context.Users.FirstOrDefaultAsync(u => u.Email == "test@example.com");
        Assert.NotNull(storedUser);
        Assert.Equal("hashedPassword", storedUser!.Password);
    }

    [Fact]
    public async Task ExistsByEmailAsync_EmailExists_ReturnsTrue()
    {
        using var context = _dbContextFactory.CreateContext();
        var repository = new UserRepository(context, _passwordHasherMock.Object);
        var user = EntityFactory.CreateUser(email: "exists@example.com");
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        var result = await repository.ExistsByEmailAsync("exists@example.com");

        Assert.True(result);
    }

    [Fact]
    public async Task ExistsByEmailAsync_EmailDoesNotExist_ReturnsFalse()
    {
        using var context = _dbContextFactory.CreateContext();
        var repository = new UserRepository(context, _passwordHasherMock.Object);

        var result = await repository.ExistsByEmailAsync("notfound@example.com");

        Assert.False(result);
    }

    [Fact]
    public async Task ExistsByIdAsync_IdExists_ReturnsTrue()
    {
        using var context = _dbContextFactory.CreateContext();
        var repository = new UserRepository(context, _passwordHasherMock.Object);
        var user = EntityFactory.CreateUser();
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        var result = await repository.ExistsByIdAsync(user.Id);

        Assert.True(result);
    }

    [Fact]
    public async Task GetByIdAsync_ValidId_ReturnsUser()
    {
        using var context = _dbContextFactory.CreateContext();
        var repository = new UserRepository(context, _passwordHasherMock.Object);
        var user = EntityFactory.CreateUser();
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        var result = await repository.GetByIdAsync(user.Id);

        Assert.NotNull(result);
        Assert.Equal(user.Email, result!.Email);
    }
}
