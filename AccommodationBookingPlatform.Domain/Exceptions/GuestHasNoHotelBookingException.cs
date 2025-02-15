namespace Domain.Exceptions;

public class GuestHasNoHotelBookingException(string message) : ConflictException(message)
{
    public override string Title => "Guest has not booked any room in the hotel";
}
