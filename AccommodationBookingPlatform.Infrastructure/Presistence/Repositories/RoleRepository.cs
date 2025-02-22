using AccommodationBookingPlatform.Domain.Entities;
using AccommodationBookingPlatform.Domain.Interfaces.Persistence.Repositories;
using AccommodationBookingPlatform.Infrastructure.Presistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class RoleRepository(HotelBookingDbContext context) : IRoleRepository
{
  public async Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
  {
    return await context.Roles
      .FirstOrDefaultAsync(r => r.Name == name, cancellationToken);
  }
}