using MediatR;

namespace AccommodationBookingPlatform.Application.Bookings.GetInvoiceAsPdf;

public class GetInvoiceAsPdfQuery : IRequest<byte[]>
{
    public Guid BookingId { get; init; }
}