using AccommodationBookingPlatform.Application.Bookings.Common;
using MediatR;

namespace AccommodationBookingPlatform.Application.Bookings.GetById;

public class GetBookingByIdQuery : IRequest<BookingResponse>
{
    public Guid BookingId { get; init; }
}