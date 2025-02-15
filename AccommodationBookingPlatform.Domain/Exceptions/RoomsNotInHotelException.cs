using AccommodationBookingPlatform.Domain.Exceptions;

namespace Domain.Exceptions;

public class RoomsNotInHotelException(string message) : BadRequestException(message)
{
  public override string Title => "Room does not exist in hotel";
}