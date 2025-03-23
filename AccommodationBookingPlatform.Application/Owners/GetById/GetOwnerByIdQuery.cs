using AccommodationBookingPlatform.Application.Owners.Common;
using MediatR;

namespace AccommodationBookingPlatform.Application.Owners.GetById;

public class GetOwnerByIdQuery : IRequest<OwnerResponse>
{
    public Guid OwnerId { get; init; }
}