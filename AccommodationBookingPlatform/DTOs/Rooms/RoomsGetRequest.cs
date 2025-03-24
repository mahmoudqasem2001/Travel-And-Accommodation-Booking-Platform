using AccommodationBookingPlatform.DTOs.Common;

namespace AccommodationBookingPlatform.DTOs.Rooms;

public class RoomsGetRequest : ResourcesQueryRequest
{
    public string? SearchTerm { get; init; }
}