using AccommodationBookingPlatform.Domain.Enums;
using AccommodationBookingPlatform.DTOs.Common;

namespace AccommodationBookingPlatform.DTOs.Hotels;

public class HotelSearchRequest : ResourcesQueryRequest
{
    public string? SearchTerm { get; init; }
    public DateOnly CheckInDateUtc { get; init; }
    public DateOnly CheckOutDateUtc { get; init; }
    public int NumberOfAdults { get; init; }
    public int NumberOfChildren { get; init; }
    public int NumberOfRooms { get; init; }
    public decimal? MinPrice { get; init; }
    public decimal? MaxPrice { get; init; }
    public int? MinStarRating { get; init; }
    public IEnumerable<RoomType>? RoomTypes { get; init; }
    public IEnumerable<Guid>? Amenities { get; init; }
}