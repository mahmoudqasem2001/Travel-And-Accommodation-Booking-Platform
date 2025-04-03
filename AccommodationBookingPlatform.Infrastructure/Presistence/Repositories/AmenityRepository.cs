using System.Linq.Expressions;
using AccommodationBookingPlatform.Domain.Entities;
using AccommodationBookingPlatform.Domain.Interfaces.Persistence.Repositories;
using AccommodationBookingPlatform.Domain.Models;
using AccommodationBookingPlatform.Infrastructure.Presistence.DbContexts;
using AccommodationBookingPlatform.Infrastructure.Presistence.Extensions;
using AccommodationBookingPlatform.Infrastructure.Presistence.Helpers;
using Domain.Exceptions;
using Domain.Messages;
using Microsoft.EntityFrameworkCore;


namespace AccommodationBookingPlatform.Infrastructure.Presistence.Repositories;

public class AmenityRepository(HotelBookingDbContext context) : IAmenityRepository
{
    public async Task<Amenity?> GetByIdAsync(Guid id,
      CancellationToken cancellationToken = default)
    {
        return await context.Amenities
          .FindAsync([id], cancellationToken);
    }

    public async Task<bool> ExistsAsync(Expression<Func<Amenity, bool>> predicate,
                                        CancellationToken cancellationToken = default)
    {
        return await context.Amenities.AnyAsync(predicate, cancellationToken);
    }

    public async Task<Amenity> CreateAsync(Amenity amenity,
      CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(amenity);

        var createdAmenity = await context.Amenities
          .AddAsync(amenity, cancellationToken);

        return createdAmenity.Entity;
    }

    public async Task UpdateAsync(Amenity amenity,
      CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(amenity);

        if (!await context.Amenities.AnyAsync(
              a => a.Id == amenity.Id, cancellationToken))
            throw new NotFoundException(AmenityMessages.WithIdNotFound);

        context.Amenities.Update(amenity);
    }

    public async Task<PaginatedList<Amenity>> GetAsync(
      Query<Amenity> query,
      CancellationToken cancellationToken = default)
    {
        var queryable = context.Amenities
          .Where(query.Filter)
          .Sort(SortingExpressions.GetAmenitySortExpression(query.SortColumn), query.SortOrder);

        var itemsToReturn = await queryable
          .GetPage(query.PageNumber, query.PageSize)
          .AsNoTracking()
          .ToListAsync(cancellationToken);

        return new PaginatedList<Amenity>(
          itemsToReturn,
          await queryable.GetPaginationMetadataAsync(
            query.PageNumber,
            query.PageSize));
    }
}