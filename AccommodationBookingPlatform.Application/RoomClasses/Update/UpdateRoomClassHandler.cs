using AutoMapper;
using Domain.Exceptions;
using Domain.Interfaces.Persistence;
using Domain.Interfaces.Persistence.Repositories;
using Domain.Messages;
using MediatR;


namespace AccommodationBookingPlatform.Application.RoomClasses.Update;

public class UpdateRoomClassHandler : IRequestHandler<UpdateRoomClassCommand>
{
    private readonly IMapper _mapper;
    private readonly IRoomClassRepository _roomClassRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateRoomClassHandler(
      IRoomClassRepository roomClassRepository,
      IUnitOfWork unitOfWork,
      IMapper mapper)
    {
        _roomClassRepository = roomClassRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task Handle(
      UpdateRoomClassCommand request,
      CancellationToken cancellationToken)
    {
        var roomClassEntity = await _roomClassRepository.GetByIdAsync(request.RoomClassId, cancellationToken);

        if (roomClassEntity is null)
        {
            throw new NotFoundException(RoomClassMessages.NotFound);
        }

        if (await _roomClassRepository.ExistsAsync(
              rc => rc.HotelId == roomClassEntity.HotelId &&
                    rc.Name == request.Name,
              cancellationToken))
        {
            throw new DuplicateRoomClassException(RoomClassMessages.NameInHotelFound);
        }

        _mapper.Map(request, roomClassEntity);

        await _roomClassRepository.UpdateAsync(roomClassEntity, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}