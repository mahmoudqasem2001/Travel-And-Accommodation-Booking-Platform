﻿
using AccommodationBookingPlatform.Domain.Entities;
using AccommodationBookingPlatform.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;


namespace Infrastructure.Presistence.Configurations;

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.HasKey(b => b.Id);

        builder.HasMany(b => b.Rooms)
          .WithMany(r => r.Bookings);

        builder.HasMany(b => b.Invoice)
          .WithOne(ir => ir.Booking)
          .IsRequired()
          .OnDelete(DeleteBehavior.Cascade);

        builder.Property(b => b.PaymentMethod)
          .HasConversion(new EnumToStringConverter<PaymentMethod>());

        builder.Property(b => b.TotalPrice)
          .HasPrecision(18, 2);

        builder.HasIndex(b => new { b.CheckInDateUtc, b.CheckOutDateUtc });
    }
}