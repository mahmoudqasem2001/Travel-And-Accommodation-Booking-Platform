using Domain.Exceptions;
using Domain.Interfaces.Persistence;
using Domain.Interfaces.Persistence.Repositories;
using Domain.Messages;
using MediatR;


namespace AccommodationBookingPlatform.Application.RoomClasses.Delete;

public class DeleteRoomClassCommandHandler : IRequestHandler<DeleteRoomClassCommand>
{
    private readonly IRoomClassRepository _roomClassRepository;
    private readonly IRoomRepository _roomRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteRoomClassCommandHandler(IRoomClassRepository roomClassRepository, IUnitOfWork unitOfWork,
      IRoomRepository roomRepository)
    {
        _roomClassRepository = roomClassRepository;
        _unitOfWork = unitOfWork;
        _roomRepository = roomRepository;
    }

    public async Task Handle(DeleteRoomClassCommand request, CancellationToken cancellationToken)
    {
        if (!await _roomClassRepository.ExistsAsync(rc => rc.Id == request.RoomClassId, cancellationToken))
        {
            throw new NotFoundException(RoomClassMessages.NotFound);
        }

        if (await _roomRepository.ExistsAsync(r => r.RoomClassId == request.RoomClassId, cancellationToken))
        {
            throw new ResourceHasDependentsException(RoomClassMessages.DependentsExist);
        }

        await _roomClassRepository.DeleteAsync(request.RoomClassId, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}