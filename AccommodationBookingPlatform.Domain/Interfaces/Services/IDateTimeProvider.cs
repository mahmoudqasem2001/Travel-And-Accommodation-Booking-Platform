namespace Domain.Interfaces.Services;

public interface IDateTimeProvider
{ 
   DateOnly GetCurrentDateUTC();

   DateTime GetCurrentDateTimeUTC();
}