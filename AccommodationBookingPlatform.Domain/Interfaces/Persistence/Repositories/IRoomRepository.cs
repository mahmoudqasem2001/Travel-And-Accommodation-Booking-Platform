using AccommodationBookingPlatform.Domain.Entities;
using AccommodationBookingPlatform.Domain.Models;
using Domain.Models;
using System.Linq.Expressions;

namespace Domain.Interfaces.Persistence.Repositories;

public interface IRoomRepository
{
    Task<bool> ExistsAsync(Expression<Func<Room, bool>> predicate,
                           CancellationToken cancellationToken = default);

    Task<PaginatedList<RoomForManagement>> GetForManagementAsync(
      Query<Room> query,
      CancellationToken cancellationToken = default);

    Task<Room?> GetByIdAsync(Guid roomClassId, Guid id, CancellationToken cancellationToken = default);

    Task<Room> CreateAsync(Room room, CancellationToken cancellationToken = default);

    Task UpdateAsync(Room room, CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    Task<PaginatedList<Room>> GetAsync(Query<Room> query, CancellationToken cancellationToken = default);

    Task<Room?> GetByIdWithRoomClassAsync(Guid roomId, CancellationToken cancellationToken = default);
}