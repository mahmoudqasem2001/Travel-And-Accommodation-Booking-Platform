
using AccommodationBookingPlatform.Domain.Entities;

namespace AccommodationBookingPlatform.Domain.Interfaces.Persistence.Repositories;

public interface IUserRepository
{
    Task<User?> AuthenticateAsync(string email, string password, CancellationToken cancellationToken = default);

    Task CreateAsync(User user, CancellationToken cancellationToken = default);

    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> ExistsByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}