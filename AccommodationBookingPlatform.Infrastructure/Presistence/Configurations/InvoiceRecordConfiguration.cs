using AccommodationBookingPlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Presistence.Configurations;

public class InvoiceRecordConfiguration : IEntityTypeConfiguration<InvoiceRecord>
{
    public void Configure(EntityTypeBuilder<InvoiceRecord> builder)
    {
        builder.HasKey(ir => ir.Id);

        builder.Property(ir => ir.PriceAtBooking)
          .HasPrecision(18, 2);

        builder.Property(ir => ir.DiscountPercentageAtBooking)
          .HasPrecision(18, 2);
    }
}