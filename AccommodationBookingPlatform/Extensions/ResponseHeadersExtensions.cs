using AccommodationBookingPlatform.Domain.Models;
using System.Text.Json;

namespace AccommodationBookingPlatform.Extensions;

public static class ResponseHeadersExtensions
{
    public static void AddPaginationMetadata(this IHeaderDictionary headers,
      PaginationMetadata paginationMetadata)
    {
        headers["x-pagination"] = JsonSerializer.Serialize(paginationMetadata);
    }
}