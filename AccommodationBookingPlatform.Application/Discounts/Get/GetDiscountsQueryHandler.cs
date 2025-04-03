using AccommodationBookingPlatform.Application.Discounts.GetById;
using AccommodationBookingPlatform.Domain.Entities;
using AccommodationBookingPlatform.Domain.Enums;
using AccommodationBookingPlatform.Domain.Models;
using AutoMapper;
using Domain.Exceptions;
using Domain.Interfaces.Persistence.Repositories;
using Domain.Messages;
using MediatR;


namespace AccommodationBookingPlatform.Application.Discounts.Get;

public class GetDiscountsQueryHandler : IRequestHandler<GetDiscountsQuery, PaginatedList<DiscountResponse>>
{
    private readonly IDiscountRepository _discountRepository;
    private readonly IMapper _mapper;
    private readonly IRoomClassRepository _roomClassRepository;

    public GetDiscountsQueryHandler(
      IRoomClassRepository roomClassRepository,
      IDiscountRepository discountRepository,
      IMapper mapper)
    {
        _roomClassRepository = roomClassRepository;
        _discountRepository = discountRepository;
        _mapper = mapper;
    }

    public async Task<PaginatedList<DiscountResponse>> Handle(
      GetDiscountsQuery request,
      CancellationToken cancellationToken = default)
    {
        if (!await _roomClassRepository.ExistsAsync(rc => rc.Id == request.RoomClassId, cancellationToken))
        {
            throw new NotFoundException(RoomClassMessages.NotFound);
        }

        var query = new Query<Discount>(
          d => d.RoomClassId == request.RoomClassId,
          request.SortOrder ?? SortOrder.Ascending,
          request.SortColumn,
          request.PageNumber,
          request.PageSize);

        var owners = await _discountRepository.GetAsync(query, cancellationToken);

        return _mapper.Map<PaginatedList<DiscountResponse>>(owners);
    }
}