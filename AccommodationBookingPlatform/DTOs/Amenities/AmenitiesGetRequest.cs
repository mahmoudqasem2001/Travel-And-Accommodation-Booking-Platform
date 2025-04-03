using AccommodationBookingPlatform.DTOs.Common;

namespace AccommodationBookingPlatform.DTOs.Amenities;

public class AmenitiesGetRequest : ResourcesQueryRequest
{
    public string? SearchTerm { get; init; }
}