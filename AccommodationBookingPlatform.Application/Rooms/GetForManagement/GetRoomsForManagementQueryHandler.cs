using System.Linq.Expressions;
using AccommodationBookingPlatform.Domain.Entities;
using AccommodationBookingPlatform.Domain.Enums;
using AccommodationBookingPlatform.Domain.Models;
using AutoMapper;
using Domain.Exceptions;
using Domain.Interfaces.Persistence.Repositories;
using Domain.Messages;
using LinqKit;
using MediatR;


namespace AccommodationBookingPlatform.Application.Rooms.GetForManagement;

public class GetRoomsHandler : IRequestHandler<GetRoomsForManagementQuery, PaginatedList<RoomForManagementResponse>>
{
    private readonly IMapper _mapper;
    private readonly IRoomClassRepository _roomClassRepository;
    private readonly IRoomRepository _roomRepository;

    public GetRoomsHandler(
      IRoomRepository roomRepository,
      IMapper mapper,
      IRoomClassRepository roomClassRepository)
    {
        _roomRepository = roomRepository;
        _mapper = mapper;
        _roomClassRepository = roomClassRepository;
    }

    public async Task<PaginatedList<RoomForManagementResponse>> Handle(GetRoomsForManagementQuery request,
      CancellationToken cancellationToken)
    {
        if (!await _roomClassRepository.ExistsAsync(
              rc => rc.Id == request.RoomClassId,
              cancellationToken))
        {
            throw new NotFoundException(RoomClassMessages.NotFound);
        }

        //var searchExpression = GetSearchExpression(request.SearchTerm);
        //Expression<Func<Room, bool>> filterExpression = r => r.RoomClassId == request.RoomClassId;

        var query = new Query<Room>(
          GetSearchExpression(request.SearchTerm)
            .And(r => r.RoomClassId == request.RoomClassId),
          request.SortOrder ?? SortOrder.Ascending,
          request.SortColumn,
          request.PageNumber,
          request.PageSize);

        var owners = await _roomRepository.GetForManagementAsync(query, cancellationToken);

        return _mapper.Map<PaginatedList<RoomForManagementResponse>>(owners);
    }

    private static Expression<Func<Room, bool>> GetSearchExpression(string? searchTerm)
    {
        return searchTerm is null
          ? _ => true
          : r => r.Number.Contains(searchTerm);
    }
}