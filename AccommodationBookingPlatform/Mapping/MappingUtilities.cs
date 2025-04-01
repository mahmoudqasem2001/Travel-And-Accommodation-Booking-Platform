using System.Data.SqlClient;

namespace AccommodationBookingPlatform.Mapping;

public static class MappingUtilities
{
    public static SortOrder? MapToSortOrder(string? sortOrder)
    {
        return sortOrder?.ToLower() switch
        {
            "asc" => SortOrder.Ascending,
            "desc" => SortOrder.Descending,
            _ => null
        };
    }
}