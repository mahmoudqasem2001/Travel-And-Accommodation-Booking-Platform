using AccommodationBookingPlatform.Domain.Exceptions;

namespace Domain.Exceptions;

public class InvalidRoleException(string message) : BadRequestException(message)
{
  public override string Title => "Invalid role";
}