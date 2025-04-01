namespace AccommodationBookingPlatform.DTOs.Discounts;

public class DiscountCreationRequest
{
    public decimal Percentage { get; init; }
    public DateTime StartDateUtc { get; init; }
    public DateTime EndDateUtc { get; init; }
}