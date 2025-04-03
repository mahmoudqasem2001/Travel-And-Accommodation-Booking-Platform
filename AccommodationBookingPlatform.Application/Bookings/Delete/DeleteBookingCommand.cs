using MediatR;

namespace AccommodationBookingPlatform.Application.Bookings.Delete;

public class DeleteBookingCommand : IRequest
{
    public Guid BookingId { get; init; }
}