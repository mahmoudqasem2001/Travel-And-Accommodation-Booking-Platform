﻿using AccommodationBookingPlatform.Application.Reviews.Common;
using AutoMapper;
using Domain.Exceptions;
using Domain.Interfaces.Persistence.Repositories;
using Domain.Messages;
using MediatR;


namespace AccommodationBookingPlatform.Application.Reviews.GetById;

public class GetReviewByIdQueryHandler : IRequestHandler<GetReviewByIdQuery, ReviewResponse>
{
    private readonly IHotelRepository _hotelRepository;
    private readonly IMapper _mapper;
    private readonly IReviewRepository _reviewRepository;

    public GetReviewByIdQueryHandler(
      IHotelRepository hotelRepository,
      IReviewRepository reviewRepository,
      IMapper mapper)
    {
        _hotelRepository = hotelRepository;
        _reviewRepository = reviewRepository;
        _mapper = mapper;
    }

    public async Task<ReviewResponse> Handle(GetReviewByIdQuery request, CancellationToken cancellationToken)
    {
        if (!await _hotelRepository.ExistsAsync(h => h.Id == request.HotelId, cancellationToken))
        {
            throw new NotFoundException(HotelMessages.NotFound);
        }

        var review = await _reviewRepository.GetByIdAsync(
                       request.HotelId, request.ReviewId, cancellationToken) ??
                     throw new NotFoundException(ReviewMessages.WithIdNotFoundInHotelWithId);

        return _mapper.Map<ReviewResponse>(review);
    }
}