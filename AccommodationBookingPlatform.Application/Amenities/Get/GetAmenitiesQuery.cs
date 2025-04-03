using AccommodationBookingPlatform.Application.Amenities.Common;
using AccommodationBookingPlatform.Domain.Enums;
using MediatR;


namespace AccommodationBookingPlatform.Application.Amenities.Get;

public class GetAmenitiesQuery : IRequest<PaginatedList<AmenityResponse>>
{
    public string? SearchTerm { get; init; }
    public SortOrder? SortOrder { get; init; }
    public string? SortColumn { get; init; }
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
}