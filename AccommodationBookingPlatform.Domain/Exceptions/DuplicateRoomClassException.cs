namespace Domain.Exceptions;

public class DuplicateRoomClassException(string message) : ConflictException(message)
{
    public override string Title => "A room class with the same name already exists in this hotel";
}
