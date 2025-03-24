using AccommodationBookingPlatform.Domain.Entities;
using AccommodationBookingPlatform.Domain.Enums;
using AccommodationBookingPlatform.Domain.Models;
using AutoMapper;
using Domain.Exceptions;
using Domain.Interfaces.Persistence.Repositories;
using Domain.Messages;
using MediatR;


namespace AccommodationBookingPlatform.Application.Rooms.GetByRoomClassIdForGuest;

public class GetRoomsByRoomClassIdForGuestsQueryHandler :
  IRequestHandler<GetRoomsByRoomClassIdForGuestsQuery,
    PaginatedList<RoomForGuestResponse>>
{
    private readonly IMapper _mapper;
    private readonly IRoomClassRepository _roomClassRepository;
    private readonly IRoomRepository _roomRepository;

    public GetRoomsByRoomClassIdForGuestsQueryHandler(IRoomClassRepository roomClassRepository,
      IRoomRepository roomRepository, IMapper mapper)
    {
        _roomClassRepository = roomClassRepository;
        _roomRepository = roomRepository;
        _mapper = mapper;
    }

    public async Task<PaginatedList<RoomForGuestResponse>> Handle(
      GetRoomsByRoomClassIdForGuestsQuery request,
      CancellationToken cancellationToken = default)
    {
        if (!await _roomClassRepository.ExistsAsync(
              rc => rc.Id == request.RoomClassId,
              cancellationToken))
        {
            throw new NotFoundException(RoomClassMessages.NotFound);
        }
        //Check if there are any overlapping bookings
        var query = new Query<Room>(
          r => r.RoomClassId == request.RoomClassId &&
               !r.Bookings.Any(b => request.CheckInDate >= b.CheckOutDateUtc
                                    || request.CheckOutDate <= b.CheckInDateUtc),
          SortOrder.Ascending,
          null,
          request.PageNumber,
          request.PageSize);

        var rooms = await _roomRepository.GetAsync(query, cancellationToken);

        return _mapper.Map<PaginatedList<RoomForGuestResponse>>(rooms);
    }
}