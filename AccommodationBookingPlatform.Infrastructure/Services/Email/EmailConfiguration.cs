using AccommodationBookingPlatform.Shared.OptionsValidation;
using Domain.Interfaces.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;


namespace AccommodationBookingPlatform.Infrastructure.Services.Email;

public static class EmailConfiguration
{
    public static IServiceCollection AddEmailInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IValidator<EmailConfig>, EmailConfigValidator>();

        services.AddOptions<EmailConfig>()
          .BindConfiguration(nameof(EmailConfig))
          .ValidateFluentValidation()
          .ValidateOnStart();

        services.AddTransient<IEmailService, EmailService>();

        return services;
    }
}