namespace Domain.Exceptions;

public class DuplicateCityPostOfficeException(string message) : ConflictException(message)
{
    public override string Title => "A city with this post office already exists";
}
