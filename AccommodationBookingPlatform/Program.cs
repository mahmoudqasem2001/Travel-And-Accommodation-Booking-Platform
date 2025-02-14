using AccommodationBookingPlatform.Application;
using Serilog;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddApplication();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseSerilogRequestLogging();

app.MapControllers();

app.Run();
