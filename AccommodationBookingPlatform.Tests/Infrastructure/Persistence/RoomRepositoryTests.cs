using System.Linq.Expressions;
using AccommodationBookingPlatform.Domain.Entities;
using AccommodationBookingPlatform.Domain.Models;
using AccommodationBookingPlatform.Infrastructure.Presistence.DbContexts;
using AccommodationBookingPlatform.Infrastructure.Presistence.Extensions;
using AccommodationBookingPlatform.Infrastructure.Presistence.Helpers;
using Domain.Exceptions;
using Domain.Interfaces.Persistence.Repositories;
using Domain.Messages;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace AccommodationBookingPlatform.Tests.Infrastructure.Persistence
{
    public class RoomRepository : IRoomRepository
    {
        private readonly HotelBookingDbContext context;

        public RoomRepository(HotelBookingDbContext context)
        {
            this.context = context;
        }

        public async Task<bool> ExistsAsync(Expression<Func<Room, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await context.Rooms.AnyAsync(predicate, cancellationToken);
        }

        public async Task<PaginatedList<RoomForManagement>> GetForManagementAsync(Query<Room> query, CancellationToken cancellationToken = default)
        {
            var currentDateUtc = DateOnly.FromDateTime(DateTime.UtcNow);

            var queryable = context.Rooms.AsQueryable()
                .Where(query.Filter)
                .Sort(SortingExpressions.GetRoomSortExpression(query.SortColumn), query.SortOrder);

            var projectedQueryable = queryable
                .Select(r => new RoomForManagement
                {
                    Id = r.Id,
                    RoomClassId = r.RoomClassId,
                    Number = r.Number,
                    IsAvailable = !r.Bookings.Any(
                        b => b.CheckInDateUtc >= currentDateUtc && b.CheckOutDateUtc <= currentDateUtc),
                    CreatedAtUtc = r.CreatedAtUtc,
                    ModifiedAtUtc = r.ModifiedAtUtc
                });

            var itemsToReturn = await projectedQueryable
                .GetPage(query.PageNumber, query.PageSize)
                .ToListAsync(cancellationToken);

            var metadata = await projectedQueryable.GetPaginationMetadataAsync(query.PageNumber, query.PageSize);

            return new PaginatedList<RoomForManagement>(itemsToReturn, metadata);
        }


        public async Task<Room?> GetByIdAsync(Guid roomClassId, Guid id, CancellationToken cancellationToken = default)
        {
            return await context.Rooms
                .FirstOrDefaultAsync(r => r.Id == id && r.RoomClassId == roomClassId, cancellationToken);
        }

        public async Task<Room> CreateAsync(Room room, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(room);

            var createdRoom = await context.Rooms.AddAsync(room, cancellationToken);

            return createdRoom.Entity;
        }

        public async Task UpdateAsync(Room room, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(room);

            if (!await context.Rooms.AnyAsync(r => r.Id == room.Id, cancellationToken))
                throw new NotFoundException(RoomMessages.NotFound);

            context.Rooms.Update(room);
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            if (!await context.Rooms.AnyAsync(r => r.Id == id, cancellationToken))
            {
                throw new NotFoundException(RoomMessages.NotFound);
            }

            var roomEntity = context.ChangeTracker.Entries<Room>()
                .FirstOrDefault(e => e.Entity.Id == id)?.Entity
                ?? new Room { Id = id };

            context.Rooms.Remove(roomEntity);
        }

        public async Task<PaginatedList<Room>> GetAsync(Query<Room> query, CancellationToken cancellationToken = default)
        {
            var queryable = context.Rooms
                .Where(query.Filter)
                .Sort(r => r.Id, query.SortOrder);

            var itemsToReturn = await queryable
                .GetPage(query.PageNumber, query.PageSize)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            var metadata = await queryable.GetPaginationMetadataAsync(query.PageNumber, query.PageSize);

            return new PaginatedList<Room>(itemsToReturn, metadata);
        }

        public async Task<Room?> GetByIdWithRoomClassAsync(Guid roomId, CancellationToken cancellationToken = default)
        {
            var currentDateTime = DateTime.UtcNow;

            return await context.Rooms
                .Include(r => r.RoomClass)
                .ThenInclude(rc => rc.Discounts.Where(d => d.StartDateUtc <= currentDateTime && d.EndDateUtc > currentDateTime))
                .FirstOrDefaultAsync(r => r.Id == roomId, cancellationToken);
        }
    }
}
