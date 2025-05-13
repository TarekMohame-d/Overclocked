using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class BrandConfiguration : IEntityTypeConfiguration<Brand>
    {
        public void Configure(EntityTypeBuilder<Brand> builder)
        {
            builder.HasKey(b => b.Id);
            builder.Property(b => b.BrandName).HasMaxLength(200).IsRequired();
            builder.Property(b => b.BrandImage).HasMaxLength(500).IsRequired();
            builder.Property(b => b.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");

            builder.HasMany(b => b.Products)
                   .WithOne(p => p.Brand)
                   .HasForeignKey(p => p.BrandId);

            // Indexes
            builder.HasIndex(b => b.BrandName).IsUnique();
        }
    }
}
