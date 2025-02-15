namespace Domain.Messages;

public static class RoomClassMessages
{
    public const string NotFound = "No room class found with the given ID.";
    public const string NameInHotelFound = "A room class with this name already exists in the specified hotel.";
    public const string DuplicatedRoomNumber = "A room with this number already exists in the specified room class.";
    public const string RoomNotFound = "The specified room does not exist in this room class.";
    public const string DependentsExist = "There are existing rooms associated with this room class.";
}
