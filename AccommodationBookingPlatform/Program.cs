using AccommodationBookingPlatform;
using AccommodationBookingPlatform.Application;
using AccommodationBookingPlatform.Infrastructure;
using Microsoft.AspNetCore.RateLimiting;
using Serilog;


var builder = WebApplication.CreateBuilder(args);


//builder.Services.AddControllers();
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

builder.Services
  .AddWebComponents()
  .AddApplication()
  .AddInfrastructure(builder.Configuration);

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
