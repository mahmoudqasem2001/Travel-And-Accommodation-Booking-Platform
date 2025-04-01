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

public class BookingRepository(HotelBookingDbContext context) : IBookingRepository
{
    public async Task<bool> ExistsAsync(Expression<Func<Booking, bool>> predicate,
                                        CancellationToken cancellationToken = default)
    {
        return await context.Bookings.AnyAsync(predicate, cancellationToken);
    }

    public async Task<Booking> CreateAsync(Booking booking, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(booking);

        var createdBooking = await context.AddAsync(booking, cancellationToken);

        return createdBooking.Entity;
    }

    public async Task<Booking?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Bookings
          .FindAsync([id], cancellationToken);
    }

    public async Task<Booking?> GetByIdAsync(Guid id, Guid guestId,
      bool includeInvoice = false,
      CancellationToken cancellationToken = default)
    {
        var bookings = context.Bookings
          .Where(b => b.Id == id && b.GuestId == guestId)
          .Include(b => b.Hotel);

        if (includeInvoice)
        {
            bookings.Include(b => b.Invoice);
        }

        return await bookings.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        if (!await context.Bookings.AnyAsync(b => b.Id == id, cancellationToken))
        {
            throw new NotFoundException(BookingMessages.NotFound);
        }

        var entity = context.ChangeTracker.Entries<Booking>()
                       .FirstOrDefault(e => e.Entity.Id == id)?.Entity
                     ?? new Booking { Id = id };

        context.Bookings.Remove(entity);
    }
    public async Task<PaginatedList<Booking>> GetAsync(Query<Booking> query,
      CancellationToken cancellationToken = default)
    {
        var queryable = context.Bookings
          .Where(query.Filter)
          .Sort(SortingExpressions.GetBookingSortExpression(query.SortColumn), query.SortOrder);

        var itemsToReturn = await queryable
          .GetPage(query.PageNumber, query.PageSize)
          .AsNoTracking()
          .ToListAsync(cancellationToken);

        return new PaginatedList<Booking>(
          itemsToReturn,
          await queryable.GetPaginationMetadataAsync(
            query.PageNumber,
            query.PageSize));
    }

    public async Task<IEnumerable<Booking>> GetRecentBookingsInDifferentHotelsByGuestId(Guid guestId,
     int count, CancellationToken cancellationToken = default)
    {
        var recentBookings = await context.Bookings
            .Where(b => b.GuestId == guestId)
            .OrderByDescending(b => b.CheckInDateUtc)
            .GroupBy(b => b.HotelId)
            .Select(g => g.First())
            .Take(count)
            .Include(b => b.Hotel)
            .ThenInclude(h => h.City)
            .Include(b => b.Hotel)
            .ThenInclude(h => h.Gallery.Where(i => i.Type == ImageType.Thumbnail))
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return recentBookings;
    }
}