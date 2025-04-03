using MediatR;

namespace AccommodationBookingPlatform.Application.Reviews.Delete;

public class DeleteReviewCommand : IRequest
{
    public Guid HotelId { get; init; }
    public Guid ReviewId { get; init; }
}