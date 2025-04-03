using AccommodationBookingPlatform.Application.Cities.Create;
using AccommodationBookingPlatform.Application.Cities.GetForManagement;
using AccommodationBookingPlatform.Application.Cities.GetTrending;
using AccommodationBookingPlatform.Application.Cities.Update;
using AccommodationBookingPlatform.Domain.Entities;
using AutoMapper;
using Domain.Models;


namespace AccommodationBookingPlatform.Application.Mapping;

public class CityProfile : Profile
{
    public CityProfile()
    {
        CreateMap<PaginatedList<CityForManagement>, PaginatedList<CityForManagementResponse>>()
          .ForMember(dst => dst.Items, options => options.MapFrom(src => src.Items));
        CreateMap<CreateCityCommand, City>();
        CreateMap<UpdateCityCommand, City>();
        CreateMap<City, CityResponse>();
        CreateMap<City, TrendingCityResponse>()
          .ForMember(dst => dst.ThumbnailUrl, options => options.MapFrom(
            src => src.Thumbnail != null ? src.Thumbnail.Path : null));
        CreateMap<CityForManagement, CityForManagementResponse>();
    }
}