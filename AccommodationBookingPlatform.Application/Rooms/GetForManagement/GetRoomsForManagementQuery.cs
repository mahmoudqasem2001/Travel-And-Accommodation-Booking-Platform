using AccommodationBookingPlatform.Domain.Enums;
using MediatR;


namespace AccommodationBookingPlatform.Application.Rooms.GetForManagement;

public class GetRoomsForManagementQuery : IRequest<PaginatedList<RoomForManagementResponse>>
{
    public Guid RoomClassId { get; init; }
    public string? SearchTerm { get; init; }
    public SortOrder? SortOrder { get; init; }
    public string? SortColumn { get; init; }
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
}