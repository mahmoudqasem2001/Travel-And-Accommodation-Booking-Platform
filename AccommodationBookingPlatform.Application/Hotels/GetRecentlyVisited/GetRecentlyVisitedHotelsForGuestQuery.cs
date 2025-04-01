using MediatR;

namespace AccommodationBookingPlatform.Application.Hotels.GetRecentlyVisited;

public class GetRecentlyVisitedHotelsForGuestQuery : IRequest<IEnumerable<RecentlyVisitedHotelResponse>>
{
    public Guid GuestId { get; init; }
    public int Count { get; init; }
}