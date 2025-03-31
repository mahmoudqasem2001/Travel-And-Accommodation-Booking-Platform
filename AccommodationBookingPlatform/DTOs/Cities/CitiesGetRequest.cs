using AccommodationBookingPlatform.DTOs.Common;

namespace AccommodationBookingPlatform.DTOs.Cities;

public class CitiesGetRequest : ResourcesQueryRequest
{
    public string? SearchTerm { get; init; }
}