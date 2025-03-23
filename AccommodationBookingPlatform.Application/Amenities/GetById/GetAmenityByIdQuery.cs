using AccommodationBookingPlatform.Application.Amenities.Common;
using MediatR;

namespace AccommodationBookingPlatform.Application.Amenities.GetById;

public class GetAmenityByIdQuery : IRequest<AmenityResponse>
{
    public Guid AmenityId { get; init; }
}