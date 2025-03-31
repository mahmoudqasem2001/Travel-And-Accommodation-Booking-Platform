using AccommodationBookingPlatform.Application.Reviews.Common;
using AccommodationBookingPlatform.Domain.Enums;
using MediatR;


namespace AccommodationBookingPlatform.Application.Reviews.GetByHotelId;

public class GetReviewsByHotelIdQuery : IRequest<PaginatedList<ReviewResponse>>
{
    public Guid HotelId { get; init; }
    public SortOrder? SortOrder { get; init; }
    public string? SortColumn { get; init; }
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
}