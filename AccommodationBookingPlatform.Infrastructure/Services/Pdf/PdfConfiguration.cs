using Domain.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AccommodationBookingPlatform.Infrastructure.Services.Pdf;

public static class PdfConfiguration
{
    public static IServiceCollection AddPdfInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IPdfService, PdfService>();

        return services;
    }
}