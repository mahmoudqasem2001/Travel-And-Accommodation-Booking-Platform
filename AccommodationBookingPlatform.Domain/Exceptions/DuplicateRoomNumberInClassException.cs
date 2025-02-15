namespace Domain.Exceptions;

public class DuplicateRoomNumberInClassException(string message) : ConflictException(message)
{
    public override string Title => "A room with this number already exists in the room class";
}
