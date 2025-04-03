using AccommodationBookingPlatform.Domain.Interfaces.Persistence.Repositories;
using AutoMapper;
using MediatR;

namespace AccommodationBookingPlatform.Application.Cities.GetTrending;

public class GetTrendingCitiesQueryHandler : IRequestHandler<GetTrendingCitiesQuery, IEnumerable<TrendingCityResponse>>
{
    private readonly ICityRepository _cityRepository;
    private readonly IMapper _mapper;

    public GetTrendingCitiesQueryHandler(ICityRepository cityRepository, IMapper mapper)
    {
        _cityRepository = cityRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<TrendingCityResponse>> Handle(GetTrendingCitiesQuery request,
      CancellationToken cancellationToken)
    {
        if (request.Count <= 0)
        {
            throw new InvalidDataException();
        }

        var cities = await _cityRepository.GetMostVisitedAsync(request.Count, cancellationToken);

        return _mapper.Map<IEnumerable<TrendingCityResponse>>(cities);
    }
}