using AccommodationBookingPlatform.Domain.Entities;
using AccommodationBookingPlatform.Domain.Enums;
using Microsoft.AspNetCore.Http;


namespace Domain.Interfaces.Persistence.Repositories;

public interface IImageRepository
{
  Task<Image> CreateAsync(IFormFile image, Guid entityId, ImageType type,
    CancellationToken cancellationToken = default);

  Task DeleteForAsync(Guid entityId, ImageType type, CancellationToken cancellationToken = default);
}