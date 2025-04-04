using AccommodationBookingPlatform.Domain.Entities;
using AutoMapper;
using Domain.Exceptions;
using Domain.Interfaces.Persistence;
using Domain.Interfaces.Persistence.Repositories;
using Domain.Messages;
using MediatR;


namespace AccommodationBookingPlatform.Application.Rooms.Create;

public class CreateRoomHandler : IRequestHandler<CreateRoomCommand, Guid>
{
    private readonly IMapper _mapper;
    private readonly IRoomClassRepository _roomClassRepository;
    private readonly IRoomRepository _roomRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateRoomHandler(
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

    public async Task<Guid> Handle(CreateRoomCommand request, CancellationToken cancellationToken)
    {
        if (!await _roomClassRepository.ExistsAsync(rc => rc.Id == request.RoomClassId, cancellationToken))
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

        var createdRoom = await _roomRepository.CreateAsync(
          _mapper.Map<Room>(request),
          cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return createdRoom.Id;
    }
}