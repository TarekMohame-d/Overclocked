using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class BrandConfiguration : IEntityTypeConfiguration<Brand>
{
    public void Configure(EntityTypeBuilder<Brand> builder)
    {
        // Attributes
        builder.HasKey(b => b.Id);
        builder.Property(b => b.Id).ValueGeneratedNever().IsRequired();
        builder.Property(b => b.Name).HasMaxLength(50).IsRequired();
        builder.Property(b => b.NormalizedName).HasMaxLength(50).HasComputedColumnSql("UPPER(\"Name\")", stored: true);
        builder.Property(b => b.Image).IsRequired();
        builder.Property(b => b.CreatedAt).HasColumnType("timestamptz").HasDefaultValueSql("NOW()");
        builder.Property(b => b.UpdatedAt).HasColumnType("timestamptz").HasDefaultValueSql("NOW()");

        // Relationships

        // Indexes
        builder.HasIndex(b => b.Name).IsUnique();
        builder.HasIndex(b => b.NormalizedName).IsUnique();
    }
}
