using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Overclocked.Domain.BrandAggregate;
using Overclocked.Domain.BrandAggregate.ValueObjects;
using Overclocked.Domain.Common.Shared.ValueObjects.Image;

namespace Overclocked.Infrastructure.Persistence.Configurations;

public class BrandConfiguration : IEntityTypeConfiguration<Brand>
{
    public void Configure(EntityTypeBuilder<Brand> builder)
    {
        builder.ToTable("brands");

        // Attributes
        builder.HasKey(b => b.Id);
        builder
            .Property(b => b.Id)
            .ValueGeneratedNever()
            .HasConversion(id => id.Value, value => BrandId.Create(value))
            .IsRequired();

        builder.Property(b => b.Name).HasMaxLength(50).IsRequired();

        builder.Property(b => b.NormalizedName).HasMaxLength(50).IsRequired();

        builder.Property(b => b.Image).HasConversion(name => name.Value, value => Image.Load(value)).IsRequired();

        builder.Property(b => b.CreatedAt).HasColumnType("timestamptz").IsRequired();

        builder.Property(b => b.UpdatedAt).HasColumnType("timestamptz").IsRequired();

        // Indexes
        builder.HasIndex(b => b.NormalizedName).IsUnique();
    }
}
