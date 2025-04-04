﻿using System.Linq.Expressions;
using AccommodationBookingPlatform.Domain.Entities;
using AccommodationBookingPlatform.Domain.Enums;
using AccommodationBookingPlatform.Domain.Models;
using AutoMapper;
using Domain.Interfaces.Persistence.Repositories;
using MediatR;


namespace AccommodationBookingPlatform.Application.RoomClasses.GetForManagement;

public class GetRoomClassesForManagementQueryHandler : IRequestHandler<GetRoomClassesForManagementQuery,
  PaginatedList<RoomClassForManagementResponse>>
{
    private readonly IMapper _mapper;
    private readonly IRoomClassRepository _roomClassRepository;

    public GetRoomClassesForManagementQueryHandler(IRoomClassRepository roomClassRepository, IMapper mapper)
    {
        _roomClassRepository = roomClassRepository;
        _mapper = mapper;
    }

    public async Task<PaginatedList<RoomClassForManagementResponse>> Handle(GetRoomClassesForManagementQuery request,
      CancellationToken cancellationToken)
    {
        var query = new Query<RoomClass>(
          GetSearchExpression(request.SearchTerm),
          request.SortOrder ?? SortOrder.Ascending,
          request.SortColumn,
          request.PageNumber,
          request.PageSize);

        var owners = await _roomClassRepository.GetAsync(
          query,
          false,
          cancellationToken);

        return _mapper.Map<PaginatedList<RoomClassForManagementResponse>>(owners);
    }

    private static Expression<Func<RoomClass, bool>> GetSearchExpression(string? searchTerm)
    {
        return searchTerm is null
          ? _ => true
          : rc => rc.Name.Contains(searchTerm);
    }
}