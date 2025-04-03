using AccommodationBookingPlatform.Domain.Interfaces.Persistence.Repositories;
using AccommodationBookingPlatform.Domain.Entities;
using AutoMapper;
using Domain.Exceptions;
using Domain.Interfaces.Persistence;
using Domain.Interfaces.Persistence.Repositories;
using Domain.Messages;
using MediatR;


namespace AccommodationBookingPlatform.Application.Hotels.Create;

public class CreateHotelCommandHandler : IRequestHandler<CreateHotelCommand, Guid>
{
    private readonly ICityRepository _cityRepository;
    private readonly IHotelRepository _hotelRepository;
    private readonly IMapper _mapper;
    private readonly IOwnerRepository _ownerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateHotelCommandHandler(
      ICityRepository cityRepository,
      IOwnerRepository ownerRepository,
      IHotelRepository hotelRepository,
      IUnitOfWork unitOfWork,
      IMapper mapper)
    {
        _cityRepository = cityRepository;
        _ownerRepository = ownerRepository;
        _hotelRepository = hotelRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Guid> Handle(CreateHotelCommand request, CancellationToken cancellationToken = default)
    {
        if (!await _cityRepository.ExistsAsync(c => c.Id == request.CityId, cancellationToken))
        {
            throw new NotFoundException(CityMessages.NotFound);
        }

        if (!await _ownerRepository.ExistsAsync(o => o.Id == request.OwnerId, cancellationToken))
        {
            throw new NotFoundException(OwnerMessages.NotFound);
        }

        if (await _hotelRepository.ExistsAsync(
              h => h.Longitude == request.Longitude && h.Longitude == request.Latitude,
              cancellationToken))
        {
            throw new DuplicateHotelLocationException(HotelMessages.SameLocationExists);
        }

        var createdHotel = await _hotelRepository.CreateAsync(
          _mapper.Map<Hotel>(request),
          cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return createdHotel.Id;
    }
}