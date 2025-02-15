using AccommodationBookingPlatform.Domain.Enums;
using System.Linq.Expressions;

namespace AccommodationBookingPlatform.Domain.Models
{
    public record Query<TEntity>(
      Expression<Func<TEntity, bool>> Filter,
      SortOrder SortOrder,
      string? SortColumn,
      int PageNumber,
      int PageSize);
}