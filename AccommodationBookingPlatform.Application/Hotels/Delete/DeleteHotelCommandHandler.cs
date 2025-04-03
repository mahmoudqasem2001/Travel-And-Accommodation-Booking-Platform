using Domain.Exceptions;
using Domain.Interfaces.Persistence;
using Domain.Interfaces.Persistence.Repositories;
using Domain.Messages;
using MediatR;

namespace AccommodationBookingPlatform.Application.Hotels.Delete;

public class DeleteHotelCommandHandler : IRequestHandler<DeleteHotelCommand>
{
    private readonly IHotelRepository _hotelRepository;
    private readonly IRoomClassRepository _roomClassRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteHotelCommandHandler(
      IHotelRepository hotelRepository,
      IUnitOfWork unitOfWork,
      IRoomClassRepository roomClassRepository)
    {
        _hotelRepository = hotelRepository;
        _unitOfWork = unitOfWork;
        _roomClassRepository = roomClassRepository;
    }

    public async Task Handle(DeleteHotelCommand request, CancellationToken cancellationToken = default)
    {
        if (!await _hotelRepository.ExistsAsync(h => h.Id == request.HotelId, cancellationToken))
        {
            throw new NotFoundException(HotelMessages.NotFound);
        }

        if (await _roomClassRepository.ExistsAsync(rc => rc.HotelId == request.HotelId, cancellationToken))
        {
            throw new ResourceHasDependentsException(HotelMessages.DependentsExist);
        }

        await _hotelRepository.DeleteAsync(request.HotelId, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}