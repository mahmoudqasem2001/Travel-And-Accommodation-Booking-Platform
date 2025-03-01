
namespace AccommodationBookingPlatform.DTOs.Common;

public class ResourcesQueryRequest
{
    private const int MaxPageSize = 20;

    private int _pageSize = 10;
    public string? SortOrder { get; init; }

    public string? SortColumn { get; init; }
    public int PageNumber { get; init; } = 1;

    public int PageSize
    {
        get => _pageSize;
        init => _pageSize = Math.Min(value, MaxPageSize);
    }
}