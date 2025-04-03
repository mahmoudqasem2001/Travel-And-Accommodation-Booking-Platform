using MediatR;

namespace AccommodationBookingPlatform.Application.Hotels.GetForGuestById;

public class GetHotelForGuestByIdQuery : IRequest<HotelForGuestResponse>
{
    public Guid HotelId { get; init; }
}