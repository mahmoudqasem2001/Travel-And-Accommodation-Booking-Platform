using AccommodationBookingPlatform.Application.Cities.Create;
using AccommodationBookingPlatform.Application.Cities.GetForManagement;
using AccommodationBookingPlatform.Application.Cities.GetTrending;
using AccommodationBookingPlatform.Application.Cities.Update;
using AccommodationBookingPlatform.DTOs.Cities;
using AutoMapper;
using static AccommodationBookingPlatform.Mapping.MappingUtilities;

namespace AccommodationBookingPlatform.Mapping;

public class CityProfile : Profile
{
    public CityProfile()
    {
        CreateMap<CitiesGetRequest, GetCitiesForManagementQuery>()
          .ForMember(dst => dst.SortOrder, opt => opt.MapFrom(src => MapToSortOrder(src.SortOrder)));

        CreateMap<TrendingCitiesGetRequest, GetTrendingCitiesQuery>();

        CreateMap<CityCreationRequest, CreateCityCommand>();

        CreateMap<CityUpdateRequest, UpdateCityCommand>();
    }
}