using AccommodationBookingPlatform.Application.Bookings.Common;
using AccommodationBookingPlatform.Domain.Enums;
using MediatR;


namespace AccommodationBookingPlatform.Application.Bookings.GetForGuest;

public class GetBookingsQuery : IRequest<PaginatedList<BookingResponse>>
{
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public SortOrder? SortOrder { get; init; }
    public string? SortColumn { get; init; }
}