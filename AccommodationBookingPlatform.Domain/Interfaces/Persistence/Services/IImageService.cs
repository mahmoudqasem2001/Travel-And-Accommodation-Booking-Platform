using AccommodationBookingPlatform.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace AccommodationBookingPlatform.Domain.Interfaces.Persistence.Services;

public interface IImageService
{
    Task<Image> StoreAsync(
      IFormFile image,
      CancellationToken cancellationToken = default);

    Task DeleteAsync(
      Image image,
      CancellationToken cancellationToken = default);
}