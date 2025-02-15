namespace Domain.Exceptions;

public class DuplicateEmailUserException(string message) : ConflictException(message)
{
    public override string Title => "A user with this email already exists";
}
