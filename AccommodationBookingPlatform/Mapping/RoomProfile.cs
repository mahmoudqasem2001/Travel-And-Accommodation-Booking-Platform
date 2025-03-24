using AccommodationBookingPlatform.Application.Rooms.Create;
using AccommodationBookingPlatform.Application.Rooms.GetByRoomClassIdForGuest;
using AccommodationBookingPlatform.Application.Rooms.GetForManagement;
using AccommodationBookingPlatform.Application.Rooms.Update;
using AccommodationBookingPlatform.DTOs.Rooms;
using AutoMapper;
using static AccommodationBookingPlatform.Mapping.MappingUtilities;

namespace AccommodationBookingPlatform.Mapping;

public class RoomProfile : Profile
{
    public RoomProfile()
    {
        CreateMap<RoomsGetRequest, GetRoomsForManagementQuery>()
          .ForMember(dst => dst.SortOrder, opt => opt.MapFrom(src => MapToSortOrder(src.SortOrder)));

        CreateMap<RoomsForGuestsGetRequest, GetRoomsByRoomClassIdForGuestsQuery>();

        CreateMap<RoomCreationRequest, CreateRoomCommand>();

        CreateMap<RoomUpdateRequest, UpdateRoomCommand>();
    }
}