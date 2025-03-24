using AccommodationBookingPlatform.Application.RoomClasses.Create;
using AccommodationBookingPlatform.Application.RoomClasses.GetByHotelIdForGuest;
using AccommodationBookingPlatform.Application.RoomClasses.GetForManagement;
using AccommodationBookingPlatform.Application.RoomClasses.Update;
using AccommodationBookingPlatform.DTOs.RoomClasses;
using AutoMapper;
using static AccommodationBookingPlatform.Mapping.MappingUtilities;

namespace AccommodationBookingPlatform.Mapping;

public class RoomClassProfile : Profile
{
    public RoomClassProfile()
    {
        CreateMap<GetRoomClassesForGuestRequest, GetRoomClassesByHotelIdForGuestQuery>()
          .ForMember(dst => dst.SortOrder, opt => opt.MapFrom(src => MapToSortOrder(src.SortOrder)));

        CreateMap<RoomClassesGetRequest, GetRoomClassesForManagementQuery>()
          .ForMember(dst => dst.SortOrder, opt => opt.MapFrom(src => MapToSortOrder(src.SortOrder)));

        CreateMap<RoomClassCreationRequest, CreateRoomClassCommand>()
          .ForMember(dst => dst.AmenitiesIds, opt => opt.MapFrom(src => src.AmenitiesIds ?? Enumerable.Empty<Guid>()));

        CreateMap<RoomClassUpdateRequest, UpdateRoomClassCommand>();
    }
}