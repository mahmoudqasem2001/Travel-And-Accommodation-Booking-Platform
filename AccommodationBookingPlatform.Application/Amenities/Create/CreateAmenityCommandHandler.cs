using AccommodationBookingPlatform.Application.Amenities.Common;
using AccommodationBookingPlatform.Domain.Entities;
using AutoMapper;
using Domain.Exceptions;
using Domain.Interfaces.Persistence;
using Domain.Interfaces.Persistence.Repositories;
using Domain.Messages;
using MediatR;


namespace AccommodationBookingPlatform.Application.Amenities.Create;

public class CreateAmenityCommandHandler : IRequestHandler<CreateAmenityCommand, AmenityResponse>
{
    private readonly IAmenityRepository _amenityRepository;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public CreateAmenityCommandHandler(
      IAmenityRepository amenityRepository,
      IUnitOfWork unitOfWork,
      IMapper mapper)
    {
        _amenityRepository = amenityRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<AmenityResponse> Handle(
      CreateAmenityCommand request,
      CancellationToken cancellationToken = default)
    {
        if (await _amenityRepository.ExistsAsync(a => a.Name == request.Name, cancellationToken))
        {
            throw new DuplicateAmenityException(AmenityMessages.WithNameExists);
        }

        var createdAmenity = await _amenityRepository.CreateAsync(
          _mapper.Map<Amenity>(request),
          cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<AmenityResponse>(createdAmenity);
    }
}