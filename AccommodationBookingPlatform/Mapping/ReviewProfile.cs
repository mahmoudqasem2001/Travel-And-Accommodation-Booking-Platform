using AccommodationBookingPlatform.Application.Reviews.Create;
using AccommodationBookingPlatform.Application.Reviews.GetByHotelId;
using AccommodationBookingPlatform.Application.Reviews.Update;
using AccommodationBookingPlatform.DTOs.Reviews;
using AutoMapper;
using static AccommodationBookingPlatform.Mapping.MappingUtilities;

namespace AccommodationBookingPlatform.Mapping;

public class ReviewProfile : Profile
{
    public ReviewProfile()
    {
        CreateMap<ReviewsGetRequest, GetReviewsByHotelIdQuery>()
          .ForMember(dst => dst.SortOrder, opt => opt.MapFrom(src => MapToSortOrder(src.SortOrder)));

        CreateMap<ReviewCreationRequest, CreateReviewCommand>();

        CreateMap<ReviewUpdateRequest, UpdateReviewCommand>();
    }
}