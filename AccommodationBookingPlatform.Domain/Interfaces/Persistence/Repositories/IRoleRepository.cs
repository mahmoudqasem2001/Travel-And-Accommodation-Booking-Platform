
using AccommodationBookingPlatform.Domain.Entities;

namespace AccommodationBookingPlatform.Domain.Interfaces.Persistence.Repositories;

public interface IRoleRepository
{
    Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
}