using MediatR;

namespace AccommodationBookingPlatform.Application.Hotels.Delete;

public class DeleteHotelCommand : IRequest
{
    public Guid HotelId { get; init; }
}