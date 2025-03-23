using System.Linq.Expressions;
using AccommodationBookingPlatform.Application.Owners.Common;
using AccommodationBookingPlatform.Domain.Entities;
using AccommodationBookingPlatform.Domain.Enums;
using AccommodationBookingPlatform.Domain.Models;
using AutoMapper;
using Domain.Interfaces.Persistence.Repositories;
using MediatR;


namespace AccommodationBookingPlatform.Application.Owners.Get;

public class GetOwnersQueryHandler : IRequestHandler<GetOwnersQuery, PaginatedList<OwnerResponse>>
{
    private readonly IMapper _mapper;
    private readonly IOwnerRepository _ownerRepository;

    public GetOwnersQueryHandler(
      IOwnerRepository ownerRepository,
      IMapper mapper)
    {
        _ownerRepository = ownerRepository;
        _mapper = mapper;
    }

    public async Task<PaginatedList<OwnerResponse>> Handle(
      GetOwnersQuery request,
      CancellationToken cancellationToken)
    {
        var query = new Query<Owner>(
          GetSearchExpression(request.SearchTerm),
          request.SortOrder ?? SortOrder.Ascending,
          request.SortColumn,
          request.PageNumber,
          request.PageSize);

        var owners = await _ownerRepository.GetAsync(query, cancellationToken);

        return _mapper.Map<PaginatedList<OwnerResponse>>(owners);
    }

    private static Expression<Func<Owner, bool>> GetSearchExpression(string? searchTerm)
    {
        return searchTerm is null
          ? _ => true
          : o => o.FirstName.Contains(searchTerm) || o.LastName.Contains(searchTerm);
    }
}