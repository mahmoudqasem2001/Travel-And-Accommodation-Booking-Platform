using AccommodationBookingPlatform.Application.Reviews.Common;
using MediatR;

namespace AccommodationBookingPlatform.Application.Reviews.Create;

public class CreateReviewCommand : IRequest<ReviewResponse>
{
    public Guid HotelId { get; init; }
    public string Content { get; init; }
    public int Rating { get; init; }
}