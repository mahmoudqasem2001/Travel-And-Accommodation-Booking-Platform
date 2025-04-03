using AccommodationBookingPlatform.Infrastructure.Auth.Jwt;
using AccommodationBookingPlatform.Infrastructure.Presistence;
using AccommodationBookingPlatform.Infrastructure.Services.Date;
using AccommodationBookingPlatform.Infrastructure.Services.Email;
using AccommodationBookingPlatform.Infrastructure.Services.Pdf;
using Domain.Interfaces.Services;
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
            services.AddPersistenceInfrastructure(config)
              .AddAuthInfrastructure()
              .AddEmailInfrastructure()
              .AddPdfInfrastructure()
              .AddTransient<IDateTimeProvider, DateTimeProvider>();

            return services;
        }
    }
}
