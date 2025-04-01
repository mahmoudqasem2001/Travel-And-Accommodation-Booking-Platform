using AccommodationBookingPlatform.Domain.Interfaces.Persistence.Services;
using AccommodationBookingPlatform.Shared.OptionsValidation;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;


namespace AccommodationBookingPlatform.Infrastructure.Presistence.Services.Images;

public static class ImageServiceConfiguration
{
    public static IServiceCollection AddImageService(this IServiceCollection services)
    {
        services.AddScoped<IValidator<FirebaseConfig>, FireBaseConfigValidator>();

        services.AddOptions<FirebaseConfig>()
          .BindConfiguration(nameof(FirebaseConfig))
          .ValidateFluentValidation()
          .ValidateOnStart();

        services.AddScoped<IImageService, FirebaseImageService>();

        return services;
    }
}