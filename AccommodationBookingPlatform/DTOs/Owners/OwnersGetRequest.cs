using AccommodationBookingPlatform.DTOs.Common;

namespace AccommodationBookingPlatform.DTOs.Owners;

public class OwnersGetRequest : ResourcesQueryRequest
{
    public string? SearchTerm { get; init; }
}