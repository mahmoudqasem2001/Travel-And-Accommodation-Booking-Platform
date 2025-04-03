using AccommodationBookingPlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Presistence.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasKey(r => r.Id);

        builder.HasMany(r => r.Users)
          .WithMany(u => u.Roles);

        builder.HasData([
          new Role
      {
        Id = new Guid("2bcc5137-1acb-4645-b43d-50201850d1fb"),
        Name = "Guest"
      },
      new Role
      {
        Id = new Guid("de4f6736-fa9a-48b3-b788-fc5506bedf08"),
        Name = "Admin"
      }
        ]);
    }
}