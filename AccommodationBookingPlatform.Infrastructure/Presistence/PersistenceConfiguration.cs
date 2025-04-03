using AccommodationBookingPlatform.Domain.Entities;
using AccommodationBookingPlatform.Domain.Interfaces.Persistence.Repositories;
using AccommodationBookingPlatform.Infrastructure.Presistence.DbContexts;
using AccommodationBookingPlatform.Infrastructure.Presistence.Repositories;
using AccommodationBookingPlatform.Infrastructure.Presistence.Services;
using AccommodationBookingPlatform.Infrastructure.Presistence.Services.Images;
using Domain.Interfaces.Persistence;
using Domain.Interfaces.Persistence.Repositories;
using Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace AccommodationBookingPlatform.Infrastructure.Presistence;

public static class PersistenceConfiguration
{
    internal static IServiceCollection AddPersistenceInfrastructure(
      this IServiceCollection services,
      IConfiguration configuration)
    {
        services.AddDbContext(configuration)
          .AddPasswordHashing()
          .AddRepositories()
          .AddImageService();

        return services;
    }

    private static IServiceCollection AddDbContext(
      this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<HotelBookingDbContext>( options =>
        {

            options.UseSqlServer(configuration.GetConnectionString("SqlServer"),
            optionsBuilder => optionsBuilder.EnableRetryOnFailure(5));
        });

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }

    private static IServiceCollection AddPasswordHashing(this IServiceCollection services)
    {
        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services
          .AddScoped<IAmenityRepository, AmenityRepository>()
          .AddScoped<IBookingRepository, BookingRepository>()
          .AddScoped<ICityRepository, CityRepository>()
          .AddScoped<IDiscountRepository, DiscountRepository>()
          .AddScoped<IHotelRepository, HotelRepository>()
          .AddScoped<IImageRepository, ImageRepository>()
          .AddScoped<IOwnerRepository, OwnerRepository>()
          .AddScoped<IRoleRepository, RoleRepository>()
          .AddScoped<IRoomClassRepository, RoomClassRepository>()
          .AddScoped<IRoomRepository, RoomRepository>()
          .AddScoped<IUserRepository, UserRepository>()
          .AddScoped<IReviewRepository, ReviewRepository>();

        return services;
    }

}