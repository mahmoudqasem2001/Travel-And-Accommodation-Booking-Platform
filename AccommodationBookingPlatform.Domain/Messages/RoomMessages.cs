namespace Domain.Messages;

public static class RoomMessages
{ 
    public const string DependentsExist = "There are active bookings for this room.";
    public const string NotFound = "No room found with the given ID.";
    public const string NotInSameHotel = "The provided rooms belong to different hotels.";
   
    public static string NotAvailable(Guid roomId)
    {
        return $"Room with ID {roomId} is unavailable for the specified time period.";
    }
}