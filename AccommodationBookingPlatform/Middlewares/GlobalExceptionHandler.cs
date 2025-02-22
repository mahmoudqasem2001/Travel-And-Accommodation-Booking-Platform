using System.Diagnostics;
using AccommodationBookingPlatform.Domain.Exceptions;
using Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace AccommodationBookingPlatform.Middlewares
{
    public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
          HttpContext httpContext, Exception exception,
          CancellationToken cancellationToken)
        {
            LogException(exception);

            var (statusCode, title, detail) = MapExceptionToProblemInformation(exception);

            await Results.Problem(
              statusCode: statusCode,
              title: title,
              detail: detail,
              extensions: new Dictionary<string, object?>
              {
                  ["traceId"] = Activity.Current?.Id ?? httpContext.TraceIdentifier
              }).ExecuteAsync(httpContext);

            return true;
        }

        private void LogException(Exception exception)
        {
            if (exception is CustomException)
            {
                logger.LogWarning(exception, exception.Message);
            }
            else
            {
                logger.LogError(exception, exception.Message);
            }
        }

        private static (int statusCode, string title, string detail)
          MapExceptionToProblemInformation(Exception exception)
        {
            if (exception is not CustomException customException)
            {
                return (
                  StatusCodes.Status500InternalServerError,
                  "Internal server error",
                  "Some internal error on the server occured."
                );
            }

            return (
              customException switch
              {
                  NotFoundException => StatusCodes.Status404NotFound,
                  ConflictException => StatusCodes.Status409Conflict,
                  UnauthorizedException => StatusCodes.Status401Unauthorized,
                  BadRequestException => StatusCodes.Status400BadRequest,
                  ForbiddenException => StatusCodes.Status403Forbidden,
                  _ => StatusCodes.Status500InternalServerError
              },
              customException.Title,
              customException.Message
            );
        }
    }
}