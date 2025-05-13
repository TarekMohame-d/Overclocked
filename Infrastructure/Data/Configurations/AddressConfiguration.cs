using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class AddressConfiguration : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Street).HasMaxLength(300).IsRequired();
            builder.Property(a => a.City).HasMaxLength(50).IsRequired();
            builder.Property(a => a.Discription).HasMaxLength(1024).IsRequired();
            builder.Property(c => c.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");

            builder.HasOne(a => a.Customer)
                .WithMany(c => c.Addresses)
                .HasForeignKey(a => a.CustomerId);

            builder.HasMany(a => a.Shipments)
                .WithOne(s => s.Address)
                .HasForeignKey(s => s.AddressId);
        }
    }
}
