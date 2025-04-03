using MediatR;

namespace AccommodationBookingPlatform.DTOs.Discounts;

public class DiscountUpdateRequest : IRequest
{
    public decimal Percentage { get; init; }
    public DateTime StartDateUtc { get; init; }
    public DateTime EndDateUtc { get; init; }
}