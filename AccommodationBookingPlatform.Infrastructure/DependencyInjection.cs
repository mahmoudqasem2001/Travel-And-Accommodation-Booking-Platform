using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AccommodationBookingPlatform.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
    this IServiceCollection services,
    IConfiguration config)
        {
            ArgumentNullException.ThrowIfNull(config);
            //services.AddPersistenceInfrastructure(config)
            //  .AddAuthInfrastructure()
            //  .AddEmailInfrastructure()
            //  .AddPdfInfrastructure()
            //  .AddTransient<IDateTimeProvider, DateTimeProvider>();

            return services;
        }
    }
}
