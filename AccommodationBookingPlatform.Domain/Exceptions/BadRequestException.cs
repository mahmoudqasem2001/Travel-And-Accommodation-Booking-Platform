using Domain.Exceptions;

namespace AccommodationBookingPlatform.Domain.Exceptions
{
    public class BadRequestException(string message) : CustomException(message)
    {
        public override string Title => "Bad Request";
    }
}