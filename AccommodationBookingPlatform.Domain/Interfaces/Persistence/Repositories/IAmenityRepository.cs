using AccommodationBookingPlatform.Domain.Entities;
using AccommodationBookingPlatform.Domain.Models;
using System.Linq.Expressions;


namespace AccommodationBookingPlatform.Domain.Interfaces.Persistence.Repositories;

public interface IAmenityRepository
{
    Task<PaginatedList<Amenity>> GetAsync(Query<Amenity> query, CancellationToken cancellationToken = default);

    Task<Amenity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(Expression<Func<Amenity, bool>> predicate, CancellationToken cancellationToken = default);

    Task<Amenity> CreateAsync(Amenity amenity, CancellationToken cancellationToken = default);

    Task UpdateAsync(Amenity amenity, CancellationToken cancellationToken = default);
}