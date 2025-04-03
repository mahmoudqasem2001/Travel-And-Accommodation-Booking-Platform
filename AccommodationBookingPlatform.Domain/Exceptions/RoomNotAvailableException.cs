using AccommodationBookingPlatform.Domain.Exceptions;

namespace Domain.Exceptions;

public class RoomNotAvailableException(string message) : BadRequestException(message)
{
  public override string Title => "Room is not available";
}