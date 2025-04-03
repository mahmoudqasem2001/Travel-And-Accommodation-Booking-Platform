using AccommodationBookingPlatform.DTOs.Common;

namespace AccommodationBookingPlatform.DTOs.RoomClasses;

public class RoomClassesGetRequest : ResourcesQueryRequest
{
    public string? SearchTerm { get; init; }
}