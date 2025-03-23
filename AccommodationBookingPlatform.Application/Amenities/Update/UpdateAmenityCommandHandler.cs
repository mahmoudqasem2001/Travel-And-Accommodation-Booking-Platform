using AutoMapper;
using Domain.Exceptions;
using Domain.Interfaces.Persistence;
using Domain.Interfaces.Persistence.Repositories;
using Domain.Messages;
using MediatR;


namespace AccommodationBookingPlatform.Application.Amenities.Update;

public class UpdateAmenityCommandHandler : IRequestHandler<UpdateAmenityCommand>
{
    private readonly IAmenityRepository _amenityRepository;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateAmenityCommandHandler(
      IAmenityRepository amenityRepository,
      IUnitOfWork unitOfWork,
      IMapper mapper)
    {
        _amenityRepository = amenityRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task Handle(
      UpdateAmenityCommand request,
      CancellationToken cancellationToken = default)
    {
        var amenityEntity = await _amenityRepository.GetByIdAsync(
                              request.AmenityId,
                              cancellationToken) ??
                            throw new NotFoundException(AmenityMessages.WithIdNotFound);

        if (!await _amenityRepository.ExistsAsync(
              a => a.Name == request.Name,
              cancellationToken))
        {
            throw new DuplicateAmenityException(AmenityMessages.WithNameExists);
        }

        _mapper.Map(request, amenityEntity);

        await _amenityRepository.UpdateAsync(
          amenityEntity,
          cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}