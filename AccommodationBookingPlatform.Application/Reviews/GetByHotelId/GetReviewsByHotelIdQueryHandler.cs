using AccommodationBookingPlatform.Application.Reviews.Common;
using AccommodationBookingPlatform.Domain.Entities;
using AccommodationBookingPlatform.Domain.Enums;
using AccommodationBookingPlatform.Domain.Models;
using AutoMapper;
using Domain.Exceptions;
using Domain.Interfaces.Persistence.Repositories;
using Domain.Messages;
using MediatR;


namespace AccommodationBookingPlatform.Application.Reviews.GetByHotelId;

public class GetReviewsByHotelIdQueryHandler : IRequestHandler<GetReviewsByHotelIdQuery, PaginatedList<ReviewResponse>>
{
    private readonly IHotelRepository _hotelRepository;
    private readonly IMapper _mapper;
    private readonly IReviewRepository _reviewRepository;

    public GetReviewsByHotelIdQueryHandler(
      IReviewRepository reviewRepository,
      IMapper mapper,
      IHotelRepository hotelRepository)
    {
        _reviewRepository = reviewRepository;
        _mapper = mapper;
        _hotelRepository = hotelRepository;
    }

    public async Task<PaginatedList<ReviewResponse>> Handle(
      GetReviewsByHotelIdQuery request,
      CancellationToken cancellationToken)
    {
        if (!await _hotelRepository.ExistsAsync(h => h.Id == request.HotelId, cancellationToken))
        {
            throw new NotFoundException(HotelMessages.NotFound);
        }

        var query = new Query<Review>(
          r => r.HotelId == request.HotelId,
          request.SortOrder ?? SortOrder.Ascending,
          request.SortColumn,
          request.PageNumber,
          request.PageSize);

        var owners = await _reviewRepository.GetAsync(query,
          cancellationToken);

        return _mapper.Map<PaginatedList<ReviewResponse>>(owners);
    }
}