using AccommodationBookingPlatform.Application.Discounts.Create;
using AccommodationBookingPlatform.Application.Discounts.GetById;
using AccommodationBookingPlatform.Domain.Entities;
using AutoMapper;


namespace AccommodationBookingPlatform.Application.Mapping;

public class DiscountProfile : Profile
{
    public DiscountProfile()
    {
        CreateMap<PaginatedList<Discount>, PaginatedList<Discount>>()
          .ForMember(dst => dst.Items, options => options.MapFrom(src => src.Items));
        CreateMap<Discount, DiscountResponse>();
        CreateMap<CreateDiscountCommand, Discount>();
    }
}