using System.Linq.Expressions;
using AccommodationBookingPlatform.Domain.Interfaces.Persistence.Repositories;
using AccommodationBookingPlatform.Domain.Entities;
using AccommodationBookingPlatform.Domain.Enums;
using AccommodationBookingPlatform.Domain.Models;
using AutoMapper;
using MediatR;


namespace AccommodationBookingPlatform.Application.Cities.GetForManagement;

public class
  GetCitiesForManagementHandler : IRequestHandler<GetCitiesForManagementQuery, PaginatedList<CityForManagementResponse>>
{
    private readonly ICityRepository _cityRepository;
    private readonly IMapper _mapper;

    public GetCitiesForManagementHandler(
      ICityRepository cityRepository,
      IMapper mapper)
    {
        _cityRepository = cityRepository;
        _mapper = mapper;
    }

    public async Task<PaginatedList<CityForManagementResponse>> Handle(
      GetCitiesForManagementQuery request,
      CancellationToken cancellationToken)
    {
        var query = new Query<City>(
          GetSearchExpression(request.SearchTerm),
          request.SortOrder ?? SortOrder.Ascending,
          request.SortColumn,
          request.PageNumber,
          request.PageSize);

        var owners = await _cityRepository.GetForManagementAsync(query, cancellationToken);

        return _mapper.Map<PaginatedList<CityForManagementResponse>>(owners);
    }

    private static Expression<Func<City, bool>> GetSearchExpression(string? searchTerm)
    {
        return searchTerm is null
          ? _ => true
          : o => o.Name.Contains(searchTerm) || o.Country.Contains(searchTerm);
    }
}