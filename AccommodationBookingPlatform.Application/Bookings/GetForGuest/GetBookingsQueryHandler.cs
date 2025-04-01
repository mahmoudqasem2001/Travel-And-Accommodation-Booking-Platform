using AccommodationBookingPlatform.Application.Bookings.Common;
using AccommodationBookingPlatform.Domain;
using AccommodationBookingPlatform.Domain.Entities;
using AccommodationBookingPlatform.Domain.Enums;
using AccommodationBookingPlatform.Domain.Interfaces.Persistence.Repositories;
using AccommodationBookingPlatform.Domain.Models;
using AutoMapper;
using Domain.Exceptions;
using Domain.Interfaces.Persistence.Repositories;
using Domain.Interfaces.Services;
using Domain.Messages;
using MediatR;


namespace AccommodationBookingPlatform.Application.Bookings.GetForGuest;

public class GetBookingsQueryHandler : IRequestHandler<GetBookingsQuery, PaginatedList<BookingResponse>>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IMapper _mapper;
    private readonly IUserRepository _userRepository;
    private readonly IUserContext _userContext;

    public GetBookingsQueryHandler(
      IUserRepository userRepository,
      IBookingRepository bookingRepository,
      IMapper mapper,
      IUserContext userContext)
    {
        _userRepository = userRepository;
        _bookingRepository = bookingRepository;
        _mapper = mapper;
        _userContext = userContext;
    }

    public async Task<PaginatedList<BookingResponse>> Handle(GetBookingsQuery request,
      CancellationToken cancellationToken = default)
    {
        if (!await _userRepository.ExistsByIdAsync(_userContext.Id, cancellationToken))
        {
            throw new NotFoundException(UserMessages.NotFound);
        }

        if (_userContext.Role != UserRoles.Guest)
        {
            throw new ForbiddenException(UserMessages.NotGuest);
        }

        var query = new Query<Booking>(
          b => b.GuestId == _userContext.Id,
          request.SortOrder ?? SortOrder.Ascending,
          request.SortColumn,
          request.PageNumber,
          request.PageSize);

        var bookings = await _bookingRepository.GetAsync(query, cancellationToken);

        return _mapper.Map<PaginatedList<BookingResponse>>(bookings);
    }
}