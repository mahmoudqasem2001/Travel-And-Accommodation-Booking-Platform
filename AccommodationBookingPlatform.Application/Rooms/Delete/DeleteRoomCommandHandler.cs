using Domain.Exceptions;
using Domain.Interfaces.Persistence;
using Domain.Interfaces.Persistence.Repositories;
using Domain.Messages;
using MediatR;


namespace AccommodationBookingPlatform.Application.Rooms.Delete;

public class DeleteRoomCommandHandler : IRequestHandler<DeleteRoomCommand>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IRoomClassRepository _roomClassRepository;
    private readonly IRoomRepository _roomRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteRoomCommandHandler(
      IRoomRepository roomRepository,
      IUnitOfWork unitOfWork,
      IRoomClassRepository roomClassRepository,
      IBookingRepository bookingRepository)
    {
        _roomRepository = roomRepository;
        _unitOfWork = unitOfWork;
        _roomClassRepository = roomClassRepository;
        _bookingRepository = bookingRepository;
    }

    public async Task Handle(DeleteRoomCommand request, CancellationToken cancellationToken)
    {
        if (!await _roomClassRepository.ExistsAsync(
              rc => rc.Id == request.RoomClassId,
              cancellationToken))
        {
            throw new NotFoundException(RoomClassMessages.NotFound);
        }

        if (!await _roomRepository.ExistsAsync(
              r => r.Id == request.RoomId && r.RoomClassId == request.RoomClassId,
              cancellationToken))
        {
            throw new NotFoundException(RoomClassMessages.RoomNotFound);
        }

        if (await _bookingRepository.ExistsAsync(b => b.Rooms.Any(r => r.Id == request.RoomId), cancellationToken))
        {
            throw new ResourceHasDependentsException(RoomMessages.DependentsExist);
        }

        await _roomRepository.DeleteAsync(request.RoomId, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}