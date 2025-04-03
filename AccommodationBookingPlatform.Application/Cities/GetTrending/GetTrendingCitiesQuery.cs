using MediatR;

namespace AccommodationBookingPlatform.Application.Cities.GetTrending;

public class GetTrendingCitiesQuery : IRequest<IEnumerable<TrendingCityResponse>>
{
    public int Count { get; init; }
}