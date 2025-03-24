using AccommodationBookingPlatform.DTOs.Common;

namespace AccommodationBookingPlatform.DTOs.Rooms;

public class RoomsForGuestsGetRequest : ResourcesQueryRequest
{
    public DateOnly CheckInDateUtc { get; init; }
    public DateOnly CheckOutDateUtc { get; init; }
}