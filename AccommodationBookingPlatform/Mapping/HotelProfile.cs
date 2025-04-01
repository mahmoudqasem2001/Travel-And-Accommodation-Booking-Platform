using AccommodationBookingPlatform.Application.Hotels.Create;
using AccommodationBookingPlatform.Application.Hotels.GetFeaturedDeals;
using AccommodationBookingPlatform.Application.Hotels.GetForGuestById.GetForManagement;
using AccommodationBookingPlatform.Application.Hotels.GetForGuestById.Search;
using AccommodationBookingPlatform.Application.Hotels.GetForGuestById.Update;
using AccommodationBookingPlatform.Application.Hotels.GetRecentlyVisited;
using AccommodationBookingPlatform.Domain.Enums;
using AccommodationBookingPlatform.DTOs.Hotels;
using AutoMapper;
using static AccommodationBookingPlatform.Mapping.MappingUtilities;

namespace AccommodationBookingPlatform.Mapping;

public class HotelProfile : Profile
{
    public HotelProfile()
    {
        CreateMap<RecentlyVisitedHotelsGetRequest, GetRecentlyVisitedHotelsForGuestQuery>();

        CreateMap<HotelsGetRequest, GetHotelsForManagementQuery>()
          .ForMember(dst => dst.SortOrder, opt => opt.MapFrom(src => MapToSortOrder(src.SortOrder)));

        CreateMap<HotelSearchRequest, SearchForHotelsQuery>()
          .ForMember(dst => dst.SortOrder, opt => opt.MapFrom(src => MapToSortOrder(src.SortOrder)))
          .ForMember(dst => dst.RoomTypes, opt => opt.MapFrom(src => src.RoomTypes ?? Enumerable.Empty<RoomType>()))
          .ForMember(dst => dst.Amenities, opt => opt.MapFrom(src => src.Amenities ?? Enumerable.Empty<Guid>()));

        CreateMap<HotelFeaturedDealsGetRequest, GetHotelFeaturedDealsQuery>();

        CreateMap<HotelCreationRequest, CreateHotelCommand>();

        CreateMap<HotelUpdateRequest, UpdateHotelCommand>();
    }
}