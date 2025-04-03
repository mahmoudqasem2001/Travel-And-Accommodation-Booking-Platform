using AccommodationBookingPlatform.DTOs.Common;

namespace AccommodationBookingPlatform.DTOs.Hotels;

public class HotelsGetRequest : ResourcesQueryRequest
{
    public string? SearchTerm { get; init; }
}