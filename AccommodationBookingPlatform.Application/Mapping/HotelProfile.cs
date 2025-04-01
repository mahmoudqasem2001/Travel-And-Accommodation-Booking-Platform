using AccommodationBookingPlatform.Application.Hotels.Create;
using AccommodationBookingPlatform.Application.Hotels.GetForGuestById;
using AccommodationBookingPlatform.Application.Hotels.GetForGuestById.GetForManagement;
using AccommodationBookingPlatform.Application.Hotels.GetForGuestById.Search;
using AccommodationBookingPlatform.Application.Hotels.GetForGuestById.Update;
using AccommodationBookingPlatform.Domain.Entities;
using AutoMapper;
using Domain.Models;

namespace AccommodationBookingPlatform.Application.Mapping;

public class HotelProfile : Profile
{
    public HotelProfile()
    {
        CreateMap<PaginatedList<HotelSearchResult>, PaginatedList<HotelSearchResultResponse>>()
          .ForMember(dst => dst.Items, options => options.MapFrom(src => src.Items));

        CreateMap<PaginatedList<HotelForManagement>, PaginatedList<HotelForManagementResponse>>()
          .ForMember(dst => dst.Items, options => options.MapFrom(src => src.Items));

        CreateMap<CreateHotelCommand, Hotel>();
        CreateMap<UpdateHotelCommand, Hotel>();

        CreateMap<Hotel, HotelForGuestResponse>()
          .ForMember(dst => dst.ThumbnailUrl, options => options.MapFrom(
            src => src.Thumbnail != null ? src.Thumbnail.Path : null))
          .ForMember(dst => dst.GalleryUrls, options => options.MapFrom(
            src => src.Gallery.Select(i => i.Path)));

        CreateMap<HotelSearchResult, HotelSearchResultResponse>()
          .ForMember(dst => dst.ThumbnailUrl, options => options.MapFrom(
            src => src.Thumbnail != null ? src.Thumbnail.Path : null));

        CreateMap<HotelForManagement, HotelForManagementResponse>()
          .ForMember(dst => dst.Owner, options => options.MapFrom(src => src.Owner));
    }
}