namespace Domain.Exceptions
{
    public class BookingCancellationNotAllowedException(string message) : ConflictException(message)
    {
        public override string Title => "Booking cancellation is not permitted";
    }
}
