using AccommodationBookingPlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
  public void Configure(EntityTypeBuilder<User> builder)
  {
    builder.HasKey(u => u.Id);

    builder.HasIndex(u => u.Email).IsUnique();
    
    builder.HasData([
      new User
      {
        Id = new Guid("ce93f771-e6b7-46b8-af59-66251bc1998f"),
        FirstName = "Admin",
        LastName = "Admin",
        Email = "admin@bookinghotel.com",
        Password = "$2y$10$otcWuBY7O5RgrB0OihWxsuLX/dxIKKtgBSRyslyVITPYemxZnrXBm"
      }
    ]);
    
    builder.HasMany(u => u.Roles)
      .WithMany(r => r.Users)
      .UsingEntity<Dictionary<string, object>>(
        "UserRole",
        j => j.HasOne<Role>().WithMany()
          .HasForeignKey("RoleId").OnDelete(DeleteBehavior.Cascade),
        j => j.HasOne<User>().WithMany()
          .HasForeignKey("UserId").OnDelete(DeleteBehavior.Cascade))
      .HasData([new Dictionary<string, object>{
        ["UserId"] = new Guid("ce93f771-e6b7-46b8-af59-66251bc1998f"), 
        ["RoleId"] = new Guid("de4f6736-fa9a-48b3-b788-fc5506bedf08")
      }]);
  }
}