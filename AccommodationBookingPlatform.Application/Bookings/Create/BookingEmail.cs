using Domain.Models;

namespace AccommodationBookingPlatform.Application.Bookings.Create;

public static class BookingEmail
{
    public static EmailRequest GetBookingEmailRequest(string toEmail,
      IEnumerable<Attachment> attachments)
    {
        return new EmailRequest(
          [toEmail],
          "Reservation is Confirmed!",
          "Thank you for reserving on our platform, here is the invoice",
          attachments);
    }
}