namespace Domain.Exceptions;

public class DuplicateHotelLocationException(string message) : ConflictException(message)
{
    public override string Title => "A hotel already exists at this location";
}
