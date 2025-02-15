using Domain.Exceptions;

namespace Domain.Exceptions;

public class ConflictException(string message) : CustomException(message)
{
  public override string Title => "Conflict";
}