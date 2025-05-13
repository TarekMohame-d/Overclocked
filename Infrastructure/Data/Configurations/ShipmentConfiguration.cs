using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class ShipmentConfiguration : IEntityTypeConfiguration<Shipment>
    {
        public void Configure(EntityTypeBuilder<Shipment> builder)
        {
            builder.HasKey(s => s.Id);
            builder.Property(s => s.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");

            builder.HasOne(s => s.Address)
                .WithMany(a => a.Shipments)
                .HasForeignKey(s => s.AddressId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(s => s.Customer)
                .WithMany(c => c.Shipments)
                .HasForeignKey(s => s.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(s => s.Orders)
                .WithOne(o => o.Shipment)
                .HasForeignKey(o => o.ShipmentId);
        }
    }
}
