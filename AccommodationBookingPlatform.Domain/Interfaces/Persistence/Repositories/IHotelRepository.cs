using AccommodationBookingPlatform.Domain.Entities;
using AccommodationBookingPlatform.Domain.Models;
using Domain.Models;
using System.Linq.Expressions;


namespace Domain.Interfaces.Persistence.Repositories;

public interface IHotelRepository
{
  Task<bool> ExistsAsync(Expression<Func<Hotel, bool>> predicate,
                         CancellationToken cancellationToken = default);
  
  Task<PaginatedList<HotelForManagement>> GetForManagementAsync(Query<Hotel> query,
    CancellationToken cancellationToken = default);

  Task<Hotel?> GetByIdAsync(Guid id, bool includeCity = false, bool includeThumbnail = false,
    bool includeGallery = false, CancellationToken cancellationToken = default);

  Task<Hotel> CreateAsync(Hotel hotel, CancellationToken cancellationToken = default);

  Task UpdateAsync(Hotel hotel, CancellationToken cancellationToken = default);

  Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

  Task<PaginatedList<HotelSearchResult>> GetForSearchAsync(Query<Hotel> query,
    CancellationToken cancellationToken = default);

  Task UpdateReviewById(Guid id, double newRating, CancellationToken cancellationToken = default);
}