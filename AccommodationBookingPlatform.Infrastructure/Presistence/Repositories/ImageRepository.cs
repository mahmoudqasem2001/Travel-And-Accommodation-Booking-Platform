using AccommodationBookingPlatform.Domain.Entities;
using AccommodationBookingPlatform.Domain.Enums;
using AccommodationBookingPlatform.Domain.Interfaces.Persistence.Services;
using AccommodationBookingPlatform.Infrastructure.Presistence.DbContexts;
using Domain.Interfaces.Persistence.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;


namespace AccommodationBookingPlatform.Infrastructure.Presistence.Repositories;

public class ImageRepository(
  HotelBookingDbContext context,
  IImageService imageService)
  : IImageRepository
{
    public async Task<Image> CreateAsync(
      IFormFile image,
      Guid entityId,
      ImageType type,
      CancellationToken cancellationToken = default)
    {
        var returnedImage = await imageService.StoreAsync(image, cancellationToken);

        var imageEntity = new Image
        {
            EntityId = entityId,
            Path = returnedImage.Path,
            Format = returnedImage.Format,
            Type = type
        };

        var createdImage = await context.Images.AddAsync(
          imageEntity,
          cancellationToken);

        return createdImage.Entity;
    }

    public async Task DeleteForAsync(
      Guid entityId,
      ImageType type,
      CancellationToken cancellationToken = default)
    {
        var images = await context.Images
          .Where(i => i.EntityId == entityId && i.Type == type)
          .ToListAsync(cancellationToken);

        foreach (var image in images)
        {
            await imageService.DeleteAsync(image, cancellationToken);

            context.Images.Remove(image);
        }
    }
}


