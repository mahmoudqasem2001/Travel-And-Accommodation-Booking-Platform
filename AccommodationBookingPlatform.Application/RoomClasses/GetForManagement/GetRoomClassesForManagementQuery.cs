using AccommodationBookingPlatform.Domain.Enums;
using MediatR;


namespace AccommodationBookingPlatform.Application.RoomClasses.GetForManagement;

public class GetRoomClassesForManagementQuery : IRequest<PaginatedList<RoomClassForManagementResponse>>
{
    public string? SearchTerm { get; init; }
    public SortOrder? SortOrder { get; init; }
    public string? SortColumn { get; init; }
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
}