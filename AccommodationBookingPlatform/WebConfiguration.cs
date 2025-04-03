using AccommodationBookingPlatform;
using AccommodationBookingPlatform.Middlewares;
using AccommodationBookingPlatform.Services;
using Domain.Interfaces.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text.Json.Serialization;

namespace AccommodationBookingPlatform
{
    public static class WebConfiguration
    {
        public static IServiceCollection AddWebComponents(
          this IServiceCollection services)
        {
            services.AddHttpContextAccessor();

            services.AddScoped<IUserContext, UserContext>();

            services.AddEndpointsApiExplorer()
              .AddSwagger();

            services.AddApiVersioning();

            services.AddProblemDetails()
              .AddExceptionHandler<GlobalExceptionHandler>();

            services.AddControllers(options => options.Filters.Add<ActivityLogFilter>())
              .AddJsonOptions(options =>
              {
                  options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                  options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
              });


            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            services.AddFluentValidation();

            services.AddAuthentication();

            services.AddAuthorization();


            return services;
        }

        private static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(setup =>
            {
                setup.SwaggerDoc("v1", new OpenApiInfo { Title = "Hotel booking API", Version = "v1" });

              

                setup.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });

                setup.AddSecurityRequirement(new OpenApiSecurityRequirement
              {
        {
          new OpenApiSecurityScheme
          {
            Reference = new OpenApiReference
            {
              Type = ReferenceType.SecurityScheme,
              Id = "Bearer"
            }
          },
          Array.Empty<string>()
        }
              });

            });

            return services;
        }

        private static IServiceCollection AddApiVersioning(this IServiceCollection services)
        {
            services.AddApiVersioning(setupAction =>
            {
                setupAction.AssumeDefaultVersionWhenUnspecified = true;

                setupAction.DefaultApiVersion = new ApiVersion(1, 0);

                setupAction.ReportApiVersions = true;

                setupAction.ApiVersionReader = new HeaderApiVersionReader("x-api-version");

            });

            return services;
        }

        private static IServiceCollection AddFluentValidation(this IServiceCollection services)
        {
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            services.AddFluentValidationAutoValidation();

            return services;
        }
    }
}