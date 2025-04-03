using AccommodationBookingPlatform.Application.Amenities.Create;
using AccommodationBookingPlatform.Application.Amenities.Get;
using AccommodationBookingPlatform.Application.Amenities.Update;
using AccommodationBookingPlatform.DTOs.Amenities;
using AutoMapper;
using static AccommodationBookingPlatform.Mapping.MappingUtilities;

namespace AccommodationBookingPlatform.Mapping;

public class AmenityProfile : Profile
{
    public AmenityProfile()
    {
        CreateMap<AmenitiesGetRequest, GetAmenitiesQuery>()
          .ForMember(dst => dst.SortOrder, opt => opt.MapFrom(src => MapToSortOrder(src.SortOrder)));

        CreateMap<AmenityCreationRequest, CreateAmenityCommand>();

        CreateMap<AmenityUpdateRequest, UpdateAmenityCommand>();
    }
}