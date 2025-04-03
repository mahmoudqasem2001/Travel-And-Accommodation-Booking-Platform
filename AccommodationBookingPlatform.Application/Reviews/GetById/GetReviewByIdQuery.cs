using AccommodationBookingPlatform.Application.Reviews.Common;
using MediatR;

namespace AccommodationBookingPlatform.Application.Reviews.GetById;

public class GetReviewByIdQuery : IRequest<ReviewResponse>
{
    public Guid HotelId { get; init; }
    public Guid ReviewId { get; init; }
}