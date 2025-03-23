using AccommodationBookingPlatform.Application.Amenities.Common;
using MediatR;

namespace AccommodationBookingPlatform.Application.Amenities.Create;

public class CreateAmenityCommand : IRequest<AmenityResponse>
{
    public string Name { get; init; }
    public string? Description { get; init; }
}