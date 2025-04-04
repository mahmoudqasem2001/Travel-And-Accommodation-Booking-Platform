﻿using AccommodationBookingPlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Presistence.Configurations;

public class OwnerConfiguration : IEntityTypeConfiguration<Owner>
{
    public void Configure(EntityTypeBuilder<Owner> builder)
    {
        builder.HasKey(o => o.Id);

        builder.HasMany(o => o.Hotels)
          .WithOne(h => h.Owner)
          .HasForeignKey(h => h.OwnerId)
          .IsRequired()
          .OnDelete(DeleteBehavior.Restrict);
    }
}