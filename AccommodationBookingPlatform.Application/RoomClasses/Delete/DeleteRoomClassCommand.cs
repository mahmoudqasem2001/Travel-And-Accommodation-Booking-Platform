using MediatR;

namespace AccommodationBookingPlatform.Application.RoomClasses.Delete;

public class DeleteRoomClassCommand : IRequest
{
    public Guid RoomClassId { get; init; }
}