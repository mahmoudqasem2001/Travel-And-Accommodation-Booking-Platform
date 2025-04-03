using AccommodationBookingPlatform.Domain.Entities;
using AccommodationBookingPlatform.Domain.Interfaces.Persistence.Repositories;
using AccommodationBookingPlatform.Infrastructure.Presistence.DbContexts;
using Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AccommodationBookingPlatform.Tests.Infrastructure.Persistence
{
    public class RoleRepositoryTests
    {
        private readonly HotelBookingDbContext _context;
        private readonly IRoleRepository _roleRepository;

        public RoleRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<HotelBookingDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

            _context = new HotelBookingDbContext(options);
            _roleRepository = new RoleRepository(_context);
        }

        [Fact]
        public async Task GetByNameAsync_ValidName_ReturnsRole()
        {
            // Arrange
            var role = new Role { Name = "Admin" };
            await _context.Roles.AddAsync(role);
            await _context.SaveChangesAsync();

            // Act
            var result = await _roleRepository.GetByNameAsync("Admin");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Admin", result.Name);
        }

        [Fact]
        public async Task GetByNameAsync_InvalidName_ReturnsNull()
        {
            // Act
            var result = await _roleRepository.GetByNameAsync("NonExistentRole");

            // Assert
            Assert.Null(result);
        }
    }
}