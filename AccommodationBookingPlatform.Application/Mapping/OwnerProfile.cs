using AccommodationBookingPlatform.Application.Owners.Common;
using AccommodationBookingPlatform.Application.Owners.Create;
using AccommodationBookingPlatform.Application.Owners.Update;
using AccommodationBookingPlatform.Domain.Entities;
using AutoMapper;


namespace AccommodationBookingPlatform.Application.Mapping;

public class OwnerProfile : Profile
{
    public OwnerProfile()
    {
        CreateMap<PaginatedList<Owner>, PaginatedList<OwnerResponse>>()
          .ForMember(dst => dst.Items, options => options.MapFrom(src => src.Items));
        CreateMap<Owner, OwnerResponse>();
        CreateMap<CreateOwnerCommand, Owner>();
        CreateMap<UpdateOwnerCommand, Owner>();
    }
}