using AccommodationBookingPlatform.Domain.Entities;
using AccommodationBookingPlatform.Domain.Enums;
using AccommodationBookingPlatform.Domain.Models;
using AccommodationBookingPlatform.Infrastructure.Presistence.DbContexts;
using AccommodationBookingPlatform.Infrastructure.Presistence.Extensions;
using AccommodationBookingPlatform.Infrastructure.Presistence.Helpers;
using Domain.Exceptions;
using Domain.Interfaces.Persistence.Repositories;
using Domain.Messages;
using Domain.Models;
using System.Data.Entity;
using System.Linq.Expressions;

namespace AccommodationBookingPlatform.Infrastructure.Presistence.Repositories;

public class CityRepository(HotelBookingDbContext context) : ICityRepository
{
    public async Task<bool> ExistsAsync(Expression<Func<City, bool>> predicate,
                                        CancellationToken cancellationToken = default)
    {
        return await context.Cities.AnyAsync(predicate, cancellationToken);
    }
    public async Task<PaginatedList<CityForManagement>> GetForManagementAsync(
      Query<City> query,
      CancellationToken cancellationToken = default)
    {
        var queryable = context.Cities
          .Where(query.Filter)
          .Sort(SortingExpressions.GetCitySortExpression(query.SortColumn), query.SortOrder)
          .Select(c => new CityForManagement
          {
              Id = c.Id,
              Country = c.Country,
              Name = c.Name,
              PostOffice = c.PostOffice,
              NumberOfHotels = c.Hotels.Count,
              CreatedAtUtc = c.CreatedAtUtc,
              ModifiedAtUtc = c.ModifiedAtUtc
          });

        var itemsToReturn = await queryable
          .GetPage(query.PageNumber, query.PageSize)
          .ToListAsync(cancellationToken);

        return new PaginatedList<CityForManagement>(
          itemsToReturn,
          await queryable.GetPaginationMetadataAsync(
            query.PageNumber,
            query.PageSize));
    }

    public async Task<City?> GetByIdAsync(Guid id,
      CancellationToken cancellationToken = default)
    {
        return await context.Cities
          .FindAsync([id], cancellationToken);
    }

    public async Task<City> CreateAsync(City city,
      CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(city);

        var createdCity = await context.Cities.AddAsync(city, cancellationToken);

        return createdCity.Entity;
    }

    public async Task DeleteAsync(Guid id,
      CancellationToken cancellationToken = default)
    {
        if (!await context.Cities.AnyAsync(r => r.Id == id, cancellationToken))
        {
            throw new NotFoundException(CityMessages.NotFound);
        }

        var entity = context.ChangeTracker.Entries<City>()
                       .FirstOrDefault(e => e.Entity.Id == id)?.Entity
                     ?? new City { Id = id };

        context.Cities.Remove(entity);
    }

    public async Task UpdateAsync(City city,
      CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(city);

        if (!await context.Cities.AnyAsync(c => c.Id == city.Id, cancellationToken))
            throw new NotFoundException(CityMessages.NotFound);

        context.Cities.Update(city);
    }

    public async Task<IEnumerable<City>> GetMostVisitedAsync(int count, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(count);

        // Get the most visited city IDs by counting bookings per city
        var mostVisitedCityIds = await context.Bookings
            .GroupBy(b => b.Hotel.CityId)
            .Select(g => new { CityId = g.Key, BookingCount = g.Count() })
            .OrderByDescending(g => g.BookingCount)
            .Take(count)
            .Select(g => g.CityId)
            .ToListAsync(cancellationToken);

        var mostVisitedCities = await context.Cities
            .Where(c => mostVisitedCityIds.Contains(c.Id))
            .Include(c => c.Thumbnail)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return mostVisitedCities;
    }
}