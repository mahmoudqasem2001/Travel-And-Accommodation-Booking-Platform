using Domain.Interfaces.Services;

namespace AccommodationBookingPlatform.Infrastructure.Services.Date;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime GetCurrentDateTimeUTC()
    {
        return DateTime.UtcNow;
    }

    public DateOnly GetCurrentDateUTC()
    {
        return DateOnly.FromDateTime(DateTime.UtcNow);
    }
}