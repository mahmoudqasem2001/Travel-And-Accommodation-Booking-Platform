using AccommodationBookingPlatform.Domain.Enums;
using MediatR;


namespace AccommodationBookingPlatform.Application.RoomClasses.GetByHotelIdForGuest;

public class GetRoomClassesByHotelIdForGuestQuery : IRequest<PaginatedList<RoomClassForGuestResponse>>
{
    public Guid HotelId { get; init; }
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public SortOrder? SortOrder { get; init; }
    public string? SortColumn { get; init; }
}