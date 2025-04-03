using AccommodationBookingPlatform.Application.Owners.Common;
using AccommodationBookingPlatform.Domain.Enums;
using MediatR;


namespace AccommodationBookingPlatform.Application.Owners.Get;

public class GetOwnersQuery : IRequest<PaginatedList<OwnerResponse>>
{
    public string? SearchTerm { get; init; }
    public SortOrder? SortOrder { get; init; }
    public string? SortColumn { get; init; }
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
}