﻿using System.Linq.Expressions;
using AccommodationBookingPlatform.Domain.Entities;
using AccommodationBookingPlatform.Domain.Models;
using AccommodationBookingPlatform.Infrastructure.Presistence.DbContexts;
using AccommodationBookingPlatform.Infrastructure.Presistence.Extensions;
using AccommodationBookingPlatform.Infrastructure.Presistence.Helpers;
using Domain.Exceptions;
using Domain.Interfaces.Persistence.Repositories;
using Domain.Messages;
using Microsoft.EntityFrameworkCore;


namespace AccommodationBookingPlatform.Infrastructure.Presistence.Repositories;

public class OwnerRepository(HotelBookingDbContext context) : IOwnerRepository
{
    public async Task<bool> ExistsAsync(Expression<Func<Owner, bool>> predicate,
                                        CancellationToken cancellationToken = default)
    {
        return await context.Owners.AnyAsync(predicate, cancellationToken);
    }
    public async Task<PaginatedList<Owner>> GetAsync(
      Query<Owner> query,
      CancellationToken cancellationToken)
    {
        var queryable = context.Owners
          .Where(query.Filter)
          .Sort(SortingExpressions.GetOwnerSortExpression(query.SortColumn), query.SortOrder);

        var itemsToReturn = await queryable
          .GetPage(query.PageNumber, query.PageSize)
          .AsNoTracking()
          .ToListAsync(cancellationToken);

        return new PaginatedList<Owner>(
          itemsToReturn,
          await queryable.GetPaginationMetadataAsync(
            query.PageNumber,
            query.PageSize));
    }

    public async Task<Owner?> GetByIdAsync(
      Guid id,
      CancellationToken cancellationToken = default)
    {
        return await context.Owners
          .FindAsync([id], cancellationToken);
    }

    public async Task<Owner> CreateAsync(
      Owner owner,
      CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(owner);

        var addedEntity = await context.AddAsync(owner, cancellationToken);

        return addedEntity.Entity;
    }

    public async Task UpdateAsync(
      Owner owner,
      CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(owner);

        if (!await context.Owners.AnyAsync(
              o => o.Id == owner.Id, cancellationToken))
            throw new NotFoundException(OwnerMessages.NotFound);

        context.Owners.Update(owner);
    }
}