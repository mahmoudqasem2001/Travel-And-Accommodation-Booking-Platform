﻿using AccommodationBookingPlatform.Application.Reviews.Common;
using AccommodationBookingPlatform.Domain;
using AccommodationBookingPlatform.Domain.Entities;
using AccommodationBookingPlatform.Domain.Interfaces.Persistence.Repositories;
using AutoMapper;
using Domain.Exceptions;
using Domain.Interfaces.Persistence;
using Domain.Interfaces.Persistence.Repositories;
using Domain.Interfaces.Services;
using Domain.Messages;
using MediatR;


namespace AccommodationBookingPlatform.Application.Reviews.Create;

public class CreateReviewCommandHandler : IRequestHandler<CreateReviewCommand, ReviewResponse>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IHotelRepository _hotelRepository;
    private readonly IMapper _mapper;
    private readonly IReviewRepository _reviewRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;
    private readonly IUserContext _userContext;

    public CreateReviewCommandHandler(
      IHotelRepository hotelRepository,
      IUserRepository userRepository,
      IReviewRepository reviewRepository,
      IBookingRepository bookingRepository,
      IUnitOfWork unitOfWork,
      IMapper mapper,
      IUserContext userContext)
    {
        _hotelRepository = hotelRepository;
        _userRepository = userRepository;
        _reviewRepository = reviewRepository;
        _bookingRepository = bookingRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _userContext = userContext;
    }

    public async Task<ReviewResponse> Handle(CreateReviewCommand request,
      CancellationToken cancellationToken = default)
    {
        if (!await _hotelRepository.ExistsAsync(h => h.Id == request.HotelId, cancellationToken))
        {
            throw new NotFoundException(HotelMessages.NotFound);
        }

        if (!await _userRepository.ExistsByIdAsync(_userContext.Id, cancellationToken))
        {
            throw new NotFoundException(UserMessages.NotFound);
        }

        if (_userContext.Role != UserRoles.Guest)
        {
            throw new ForbiddenException(UserMessages.NotGuest);
        }

        if (!await _bookingRepository.ExistsAsync(
              b => b.HotelId == request.HotelId && b.GuestId == _userContext.Id,
              cancellationToken))
        {
            throw new GuestHasNoHotelBookingException(BookingMessages.NoBookingForGuestInHotel);
        }

        if (await _reviewRepository.ExistsAsync(
              r => r.GuestId == _userContext.Id && r.HotelId == request.HotelId,
              cancellationToken))
        {
            throw new DuplicateReviewException(ReviewMessages.GuestAlreadyReviewedHotel);
        }

        var ratingSum = await _reviewRepository.GetTotalRatingForHotelAsync(request.HotelId, cancellationToken);

        var reviewsCount = await _reviewRepository.GetReviewCountForHotelAsync(request.HotelId, cancellationToken);

        ratingSum += request.Rating;
        reviewsCount++;

        var newRating = 1.0 * ratingSum / reviewsCount;

        await _hotelRepository.UpdateReviewById(request.HotelId, newRating, cancellationToken);

        var createdReview = await _reviewRepository
          .CreateAsync(_mapper.Map<Review>(request), cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<ReviewResponse>(createdReview);
    }
}