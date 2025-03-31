using AccommodationBookingPlatform.Application.Reviews.Common;
using AccommodationBookingPlatform.Application.Reviews.Create;
using AccommodationBookingPlatform.Application.Reviews.Update;
using AccommodationBookingPlatform.Domain.Entities;
using AutoMapper;


namespace AccommodationBookingPlatform.Application.Mapping;

public class ReviewProfile : Profile
{
    public ReviewProfile()
    {
        CreateMap<CreateReviewCommand, Review>();
        CreateMap<UpdateReviewCommand, Review>();
        CreateMap<Review, ReviewResponse>();
        CreateMap<PaginatedList<Review>, PaginatedList<ReviewResponse>>()
          .ForMember(dst => dst.Items, options => options.MapFrom(src => src.Items));
    }
}