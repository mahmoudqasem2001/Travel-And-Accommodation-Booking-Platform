using AccommodationBookingPlatform.Infrastructure.Presistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace AccommodationBookingPlatform.Tests.Common;

public class InMemoryDbContextFactory
{
    public HotelBookingDbContext CreateContext(string? databaseName = null)
    {
        var options = new DbContextOptionsBuilder<HotelBookingDbContext>()
            .UseInMemoryDatabase(databaseName ?? $"HotelBookingTests_{Guid.NewGuid()}")
            .Options;

        var context = new HotelBookingDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }
}
