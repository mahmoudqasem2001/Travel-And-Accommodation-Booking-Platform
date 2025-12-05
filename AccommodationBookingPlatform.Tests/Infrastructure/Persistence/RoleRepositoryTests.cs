using AccommodationBookingPlatform.Domain.Entities;
using AccommodationBookingPlatform.Infrastructure.Presistence.Repositories;
using AccommodationBookingPlatform.Tests.Common;
using Microsoft.EntityFrameworkCore;

namespace AccommodationBookingPlatform.Tests.Infrastructure.Persistence;

public class RoleRepositoryTests
{
    private readonly InMemoryDbContextFactory _dbContextFactory = new();

    [Fact]
    public async Task GetByNameAsync_ValidName_ReturnsRole()
    {
        using var context = _dbContextFactory.CreateContext();
        var role = new Role { Name = "Admin" };
        context.Roles.Add(role);
        await context.SaveChangesAsync();

        var repository = new RoleRepository(context);

        var result = await repository.GetByNameAsync("Admin");

        Assert.NotNull(result);
        Assert.Equal("Admin", result!.Name);
    }

    [Fact]
    public async Task GetByNameAsync_InvalidName_ReturnsNull()
    {
        using var context = _dbContextFactory.CreateContext();
        var repository = new RoleRepository(context);

        var result = await repository.GetByNameAsync("NonExistentRole");

        Assert.Null(result);
    }
}
