using AccommodationBookingPlatform.Application.Bookings.Common;
using AccommodationBookingPlatform.Domain.Enums;
using MediatR;

namespace AccommodationBookingPlatform.Application.Bookings.Create;

public class CreateBookingCommand : IRequest<BookingResponse>
{
    public IEnumerable<Guid> RoomIds { get; init; }
    public Guid HotelId { get; init; }
    public DateOnly CheckInDateUtc { get; init; }
    public DateOnly CheckOutDateUtc { get; init; }
    public string? GuestRemarks { get; init; }
    public PaymentMethod PaymentMethod { get; init; }
}