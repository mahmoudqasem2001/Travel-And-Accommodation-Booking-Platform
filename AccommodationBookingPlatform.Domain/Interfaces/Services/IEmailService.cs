using Domain.Models;

namespace Domain.Interfaces.Services;

public interface IEmailService
{
  Task SendAsync(EmailRequest emailRequest, CancellationToken cancellationToken = default);
}