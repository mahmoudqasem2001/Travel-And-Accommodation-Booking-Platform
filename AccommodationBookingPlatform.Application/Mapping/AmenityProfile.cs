using AccommodationBookingPlatform.Application.Amenities.Common;
using AccommodationBookingPlatform.Application.Amenities.Create;
using AccommodationBookingPlatform.Application.Amenities.Update;
using AccommodationBookingPlatform.Domain.Entities;
using AutoMapper;


namespace AccommodationBookingPlatform.Application.Mapping;

public class AmenityProfile : Profile
{
    public AmenityProfile()
    {
        CreateMap<PaginatedList<Amenity>, PaginatedList<AmenityResponse>>()
          .ForMember(dst => dst.Items, options => options.MapFrom(src => src.Items));
        CreateMap<Amenity, AmenityResponse>();
        CreateMap<UpdateAmenityCommand, Amenity>();
        CreateMap<CreateAmenityCommand, Amenity>();
    }
}