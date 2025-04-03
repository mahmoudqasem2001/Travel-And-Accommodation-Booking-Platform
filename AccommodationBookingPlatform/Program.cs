using AccommodationBookingPlatform;
using AccommodationBookingPlatform.Application;
using AccommodationBookingPlatform.Domain.Interfaces.Persistence.Services;
using AccommodationBookingPlatform.Infrastructure;
using Domain.Interfaces.Services;
using Microsoft.AspNetCore.RateLimiting;
using Serilog;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

builder.Services
  .AddWebComponents()
  .AddApplication()
  .AddInfrastructure(builder.Configuration);

var serviceProvider = builder.Services.BuildServiceProvider();
var registeredServices = serviceProvider.GetServices<IEmailService>();
Console.WriteLine($"---------------------------------Registered IDateTimeProvider count: {registeredServices.Count()}");

var app = builder.Build();




if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();


app.MapControllers();

app.Run();
