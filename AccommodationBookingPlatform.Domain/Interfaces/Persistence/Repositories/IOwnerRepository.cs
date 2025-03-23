using AccommodationBookingPlatform.Domain.Entities;
using AccommodationBookingPlatform.Domain.Models;
using System.Linq.Expressions;


namespace Domain.Interfaces.Persistence.Repositories;

public interface IOwnerRepository
{
  Task<bool> ExistsAsync(Expression<Func<Owner, bool>> predicate,
                         CancellationToken cancellationToken = default);
  
  Task<PaginatedList<Owner>> GetAsync(Query<Owner> query, CancellationToken cancellationToken = default);

  Task<Owner?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

  Task<Owner> CreateAsync(Owner owner, CancellationToken cancellationToken = default);

  Task UpdateAsync(Owner owner, CancellationToken cancellationToken = default);
}