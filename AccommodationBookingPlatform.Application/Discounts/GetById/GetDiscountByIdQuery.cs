using MediatR;

namespace AccommodationBookingPlatform.Application.Discounts.GetById;

public class GetDiscountByIdQuery : IRequest<DiscountResponse>
{
    public Guid RoomClassId { get; init; }
    public Guid DiscountId { get; init; }
}