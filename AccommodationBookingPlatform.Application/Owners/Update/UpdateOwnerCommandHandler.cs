using AutoMapper;
using Domain.Exceptions;
using Domain.Interfaces.Persistence;
using Domain.Interfaces.Persistence.Repositories;
using Domain.Messages;
using MediatR;


namespace AccommodationBookingPlatform.Application.Owners.Update;

public class UpdateOwnerCommandHandler : IRequestHandler<UpdateOwnerCommand>
{
    private readonly IMapper _mapper;
    private readonly IOwnerRepository _ownerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateOwnerCommandHandler(
      IOwnerRepository ownerRepository,
      IUnitOfWork unitOfWork,
      IMapper mapper)
    {
        _ownerRepository = ownerRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task Handle(UpdateOwnerCommand request, CancellationToken cancellationToken = default)
    {
        var ownerEntity = await _ownerRepository.GetByIdAsync(request.OwnerId, cancellationToken)
                          ?? throw new NotFoundException(OwnerMessages.NotFound);

        _mapper.Map(request, ownerEntity);

        await _ownerRepository.UpdateAsync(ownerEntity, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}