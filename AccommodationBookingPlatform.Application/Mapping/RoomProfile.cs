using AccommodationBookingPlatform.Application.Rooms.Create;
using AccommodationBookingPlatform.Application.Rooms.GetByRoomClassIdForGuest;
using AccommodationBookingPlatform.Application.Rooms.GetForManagement;
using AccommodationBookingPlatform.Application.Rooms.Update;
using AccommodationBookingPlatform.Domain.Entities;
using AutoMapper;
using Domain.Models;


namespace AccommodationBookingPlatform.Application.Mapping;

public class RoomProfile : Profile
{
    public RoomProfile()
    {
        CreateMap<PaginatedList<Room>, PaginatedList<RoomForGuestResponse>>()
          .ForMember(dst => dst.Items, options => options.MapFrom(src => src.Items));
        CreateMap<PaginatedList<RoomForManagement>, PaginatedList<RoomForManagementResponse>>()
          .ForMember(dst => dst.Items, options => options.MapFrom(src => src.Items));
        CreateMap<CreateRoomCommand, Room>();
        CreateMap<UpdateRoomCommand, Room>();
        CreateMap<Room, RoomForGuestResponse>();
        CreateMap<RoomForManagement, RoomForManagementResponse>();
    }
}