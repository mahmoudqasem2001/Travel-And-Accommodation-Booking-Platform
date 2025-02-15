namespace Domain.Messages;
public static class BookingMessages
{  
    public const string NotFoundForGuest = "No booking found with the provided ID for the specified guest.";
    public const string NoBookingForGuestInHotel =
      "The specified guest has not made any bookings at the specified hotel.";
    public const string NotFound = "No booking found with the provided ID.";
    public const string CheckedIn = "Booking cancellation is not allowed as the check-in date has passed.";
  
}