using AccommodationBookingPlatform.Application.Discounts.GetById;
using AccommodationBookingPlatform.Domain.Enums;
using MediatR;


namespace AccommodationBookingPlatform.Application.Discounts.Get;

public class GetDiscountsQuery : IRequest<PaginatedList<DiscountResponse>>
{
    public Guid RoomClassId { get; init; }
    public SortOrder? SortOrder { get; init; }
    public string? SortColumn { get; init; }
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
}