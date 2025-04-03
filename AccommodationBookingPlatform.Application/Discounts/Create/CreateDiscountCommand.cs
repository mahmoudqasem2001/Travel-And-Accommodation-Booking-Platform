using AccommodationBookingPlatform.Application.Discounts.GetById;
using MediatR;

namespace AccommodationBookingPlatform.Application.Discounts.Create;

public class CreateDiscountCommand : IRequest<DiscountResponse>
{
    public Guid RoomClassId { get; init; }
    public decimal Percentage { get; init; }
    public DateTime StartDateUtc { get; init; }
    public DateTime EndDateUtc { get; init; }
}