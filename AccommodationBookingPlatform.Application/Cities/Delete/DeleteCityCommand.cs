using MediatR;

namespace AccommodationBookingPlatform.Application.Cities.Delete;

public class DeleteCityCommand : IRequest
{
    public Guid CityId { get; init; }
}