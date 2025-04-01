using AccommodationBookingPlatform.Application.Bookings.Common;
using AccommodationBookingPlatform.Domain;
using AccommodationBookingPlatform.Domain.Interfaces.Persistence.Repositories;
using AutoMapper;
using Domain.Exceptions;
using Domain.Interfaces.Persistence.Repositories;
using Domain.Interfaces.Services;
using Domain.Messages;
using MediatR;


namespace AccommodationBookingPlatform.Application.Bookings.GetById;

public class GetBookingByIdQueryHandler : IRequestHandler<GetBookingByIdQuery, BookingResponse>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IMapper _mapper;
    private readonly IUserRepository _userRepository;
    private readonly IUserContext _userContext;

    public GetBookingByIdQueryHandler(
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

    public async Task<BookingResponse> Handle(
      GetBookingByIdQuery request,
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

        var booking = await _bookingRepository.GetByIdAsync(
                        _userContext.Id,
                        request.BookingId,
                        false,
                        cancellationToken) ??
                      throw new NotFoundException(BookingMessages.NotFoundForGuest);

        return _mapper.Map<BookingResponse>(booking);
    }
}