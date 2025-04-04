﻿using AccommodationBookingPlatform.Domain.Entities;
using AccommodationBookingPlatform.Domain.Models;
using System.Linq.Expressions;


namespace Domain.Interfaces.Persistence.Repositories;

public interface IDiscountRepository
{
  Task<bool> ExistsAsync(Expression<Func<Discount, bool>> predicate,
                         CancellationToken cancellationToken = default);
  Task<Discount?> GetByIdAsync(Guid roomClassId, Guid id, CancellationToken cancellationToken);

  Task<Discount> CreateAsync(Discount discount, CancellationToken cancellationToken = default);
  
  Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

  Task<PaginatedList<Discount>> GetAsync(Query<Discount> query,
    CancellationToken cancellationToken = default);
}