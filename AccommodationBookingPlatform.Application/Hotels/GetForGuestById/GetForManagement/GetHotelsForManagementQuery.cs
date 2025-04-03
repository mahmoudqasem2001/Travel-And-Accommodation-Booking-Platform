using AccommodationBookingPlatform.Domain.Enums;
using MediatR;


namespace AccommodationBookingPlatform.Application.Hotels.GetForGuestById.GetForManagement;

public class GetHotelsForManagementQuery : IRequest<PaginatedList<HotelForManagementResponse>>
{
    public string? SearchTerm { get; init; }
    public SortOrder? SortOrder { get; init; }
    public string? SortColumn { get; init; }
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
}