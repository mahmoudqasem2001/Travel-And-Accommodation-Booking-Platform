using AutoMapper;
using Domain.Exceptions;
using Domain.Interfaces.Persistence;
using Domain.Interfaces.Persistence.Repositories;
using Domain.Messages;
using MediatR;


namespace AccommodationBookingPlatform.Application.Rooms.Update;

public class UpdateRoomCommandHandler : IRequestHandler<UpdateRoomCommand>
{
    private readonly IMapper _mapper;
    private readonly IRoomClassRepository _roomClassRepository;
    private readonly IRoomRepository _roomRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateRoomCommandHandler(
      IRoomRepository roomRepository,
      IRoomClassRepository roomClassRepository,
      IMapper mapper,
      IUnitOfWork unitOfWork)
    {
        _roomRepository = roomRepository;
        _roomClassRepository = roomClassRepository;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdateRoomCommand request, CancellationToken cancellationToken)
    {
        if (!await _roomClassRepository.ExistsAsync(
              rc => rc.Id == request.RoomClassId,
              cancellationToken))
        {
            throw new NotFoundException(RoomClassMessages.NotFound);
        }

        if (await _roomRepository.ExistsAsync(
              r => r.RoomClassId == request.RoomClassId &&
                   r.Number == request.Number,
              cancellationToken))
        {
            throw new DuplicateRoomClassException(RoomClassMessages.DuplicatedRoomNumber);
        }

        var roomEntity = await _roomRepository.GetByIdAsync(
          request.RoomClassId, request.RoomId,
          cancellationToken);

        if (roomEntity is null)
        {
            throw new NotFoundException(RoomClassMessages.RoomNotFound);
        }

        _mapper.Map(request, roomEntity);

        await _roomRepository.UpdateAsync(roomEntity, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}