using AccommodationBookingPlatform.Application.Bookings.Create;
using AccommodationBookingPlatform.Application.Bookings.GetForGuest;
using AccommodationBookingPlatform.DTOs.Bookings;
using AutoMapper;

using static AccommodationBookingPlatform.Mapping.MappingUtilities;

namespace AccommodationBookingPlatform.Mapping;

public class BookingProfile : Profile
{
    public BookingProfile()
    {
        CreateMap<BookingCreationRequest, CreateBookingCommand>();

        CreateMap<BookingsGetRequest, GetBookingsQuery>()
          .ForMember(dst => dst.SortOrder, opt => opt.MapFrom(src => MapToSortOrder(src.SortOrder)));
    }
}