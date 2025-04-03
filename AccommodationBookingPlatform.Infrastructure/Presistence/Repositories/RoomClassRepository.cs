using System.Linq.Expressions;
using AccommodationBookingPlatform.Domain.Entities;
using AccommodationBookingPlatform.Domain.Enums;
using AccommodationBookingPlatform.Domain.Models;
using AccommodationBookingPlatform.Infrastructure.Presistence.DbContexts;
using AccommodationBookingPlatform.Infrastructure.Presistence.Extensions;
using AccommodationBookingPlatform.Infrastructure.Presistence.Helpers;
using Domain.Exceptions;
using Domain.Interfaces.Persistence.Repositories;
using Domain.Messages;
using Microsoft.EntityFrameworkCore;


namespace AccommodationBookingPlatform.Infrastructure.Presistence.Repositories;

public class RoomClassRepository(HotelBookingDbContext context) : IRoomClassRepository
{
    public async Task<bool> ExistsAsync(Expression<Func<RoomClass, bool>> predicate,
                                        CancellationToken cancellationToken = default)
    {
        return await context.RoomClasses.AnyAsync(predicate, cancellationToken);
    }

    public async Task<RoomClass?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.RoomClasses
          .FirstOrDefaultAsync(rc => rc.Id == id, cancellationToken);
    }

    public async Task<RoomClass> CreateAsync(RoomClass roomClass, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(roomClass);

        var createdRoomClass = await context.RoomClasses.AddAsync(roomClass, cancellationToken);

        return createdRoomClass.Entity;
    }

    public async Task UpdateAsync(RoomClass roomClass, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(roomClass);

        if (!await ExistsAsync(rc => rc.Id == roomClass.Id, cancellationToken))
            throw new NotFoundException(RoomClassMessages.NotFound);

        context.RoomClasses.Update(roomClass);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (!await context.RoomClasses.AnyAsync(r => r.Id == id, cancellationToken))
        {
            throw new NotFoundException(RoomClassMessages.NotFound);
        }

        var entity = context.ChangeTracker.Entries<RoomClass>()
                       .FirstOrDefault(e => e.Entity.Id == id)?.Entity
                     ?? new RoomClass { Id = id };

        context.RoomClasses.Remove(entity);
    }

    public async Task<PaginatedList<RoomClass>> GetAsync(
      Query<RoomClass> query,
      bool includeGallery = false,
      CancellationToken cancellationToken = default)
    {
        var currentDateTime = DateTime.UtcNow;

        var queryable = context.RoomClasses
          .Include(rc => rc.Discounts
            .Where(d => currentDateTime >= d.StartDateUtc && currentDateTime < d.EndDateUtc))
          .Include(rc => rc.Amenities)
          .AsSplitQuery()
          .Where(query.Filter)
          .Sort(SortingExpressions.GetRoomClassSortExpression(query.SortColumn), query.SortOrder);

        var requestedPage = queryable.GetPage(query.PageNumber, query.PageSize);

        IEnumerable<RoomClass> itemsToReturn;

        if (includeGallery)
        {
            itemsToReturn = (await requestedPage.Select(rc => new
            {
                RoomClass = rc,
                Gallery = context.Images
                .Where(i => i.EntityId == rc.Id && i.Type == ImageType.Gallery)
                .ToList()
            }).AsNoTracking().ToListAsync(cancellationToken))
            .Select(rc =>
            {
                rc.RoomClass.Gallery = rc.Gallery;

                return rc.RoomClass;
            });
        }
        else
        {
            itemsToReturn = await requestedPage
              .AsNoTracking()
              .ToListAsync(cancellationToken);
        }

        return new PaginatedList<RoomClass>(
          itemsToReturn,
          await queryable.GetPaginationMetadataAsync(
            query.PageNumber,
            query.PageSize));
    }

    public async Task<IEnumerable<RoomClass>> GetFeaturedDealsInDifferentHotelsAsync(int count, CancellationToken cancellationToken = default)
    {
        var currentDateTime = DateTime.UtcNow;

        var activeDiscounts = await context.Discounts
            .Where(d => d.StartDateUtc <= currentDateTime && d.EndDateUtc > currentDateTime)
            .OrderByDescending(d => d.Percentage)
            .ToListAsync(cancellationToken);

        var featuredRoomClasses = new List<RoomClass>();

        var hotels = await context.Hotels
            .Include(h => h.City)
            .Include(h => h.Gallery.Where(i => i.Type == ImageType.Thumbnail))
            .ToListAsync(cancellationToken);

        foreach (var hotel in hotels)
        {
            var bestRoomClass = await context.RoomClasses
                .Where(rc => rc.HotelId == hotel.Id)
                .OrderBy(rc => rc.PricePerNight)
                .FirstOrDefaultAsync(cancellationToken);

            if (bestRoomClass != null)
            {
                var discount = activeDiscounts.FirstOrDefault(d => d.RoomClassId == bestRoomClass.Id);
                if (discount != null)
                {
                    bestRoomClass.Discounts.Add(discount);
                }

                bestRoomClass.Hotel = hotel;
                featuredRoomClasses.Add(bestRoomClass);
            }

            if (featuredRoomClasses.Count >= count)
            {
                break;
            }
        }

        return featuredRoomClasses;
    }

}