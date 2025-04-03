using MediatR;

namespace AccommodationBookingPlatform.Application.Rooms.Delete;

public class DeleteRoomCommand : IRequest
{
    public Guid RoomClassId { get; init; }
    public Guid RoomId { get; init; }
}