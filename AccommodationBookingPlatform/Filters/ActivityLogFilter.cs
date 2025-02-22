using AccommodationBookingPlatform.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;


public class ActivityLogFilter(ILogger<ActivityLogFilter> logger) : IAsyncActionFilter
{
  public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
  {
    // No Logging for sensitive data.
    if (context.Controller.GetType() == typeof(AuthController))
    {
      logger.LogInformation(
        "Executing {$ActionMethodName} on controller {$ControllerName}",
        context.ActionDescriptor.DisplayName,
        context.Controller);

      await next();

      logger.LogInformation(
        "Action {$ActionMethodName} finished execution on controller {$ControllerName}",
        context.ActionDescriptor.DisplayName,
        context.Controller);

      return;
    }

    logger.LogInformation(
      "Executing {$ActionMethodName} on controller {$ControllerName}, with arguments {@ActionArguments}",
      context.ActionDescriptor.DisplayName,
      context.Controller,
      context.ActionArguments.Where(o => o.Value is not CancellationToken));

    await next();

    logger.LogInformation(
      "Action {$ActionMethodName} finished execution on controller {$ControllerName}, with arguments {@ActionArguments}",
      context.ActionDescriptor.DisplayName,
      context.Controller,
      context.ActionArguments.Where(o => o.Value is not CancellationToken));
  }
}