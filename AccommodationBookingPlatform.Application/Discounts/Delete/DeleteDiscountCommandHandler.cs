﻿using Domain.Exceptions;
using Domain.Interfaces.Persistence;
using Domain.Interfaces.Persistence.Repositories;
using Domain.Messages;
using MediatR;

namespace AccommodationBookingPlatform.Application.Discounts.Delete;

public class DeleteDiscountCommandHandler : IRequestHandler<DeleteDiscountCommand>
{
    private readonly IDiscountRepository _discountRepository;
    private readonly IRoomClassRepository _roomClassRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteDiscountCommandHandler(
      IRoomClassRepository roomClassRepository,
      IDiscountRepository discountRepository,
      IUnitOfWork unitOfWork)
    {
        _roomClassRepository = roomClassRepository;
        _discountRepository = discountRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteDiscountCommand request, CancellationToken cancellationToken)
    {
        if (!await _roomClassRepository.ExistsAsync(
              rc => rc.Id == request.RoomClassId,
              cancellationToken))
        {
            throw new NotFoundException(RoomClassMessages.NotFound);
        }

        if (!await _discountRepository.ExistsAsync(
              d => d.Id == request.DiscountId && d.RoomClassId == request.RoomClassId,
              cancellationToken))
        {
            throw new NotFoundException(DiscountMessages.NotFoundInRoomClass);
        }

        await _discountRepository.DeleteAsync(request.DiscountId, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}