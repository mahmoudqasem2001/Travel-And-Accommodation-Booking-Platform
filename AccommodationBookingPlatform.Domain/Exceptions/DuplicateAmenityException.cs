
namespace Domain.Exceptions;

public class DuplicateAmenityException(string message) : ConflictException(message)
{
    public override string Title => "Amenity is already registered";
}
