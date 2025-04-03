using AccommodationBookingPlatform.Application.Discounts.Create;
using AccommodationBookingPlatform.Application.Discounts.Get;
using AccommodationBookingPlatform.DTOs.Discounts;
using AutoMapper;
using static AccommodationBookingPlatform.Mapping.MappingUtilities;

namespace AccommodationBookingPlatform.Mapping;

public class DiscountProfile : Profile
{
    public DiscountProfile()
    {
        CreateMap<DiscountsGetRequest, GetDiscountsQuery>()
          .ForMember(dst => dst.SortOrder, opt => opt.MapFrom(src => MapToSortOrder(src.SortOrder)));

        CreateMap<DiscountCreationRequest, CreateDiscountCommand>();
    }
}