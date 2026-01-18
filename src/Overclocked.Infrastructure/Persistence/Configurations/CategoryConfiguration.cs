using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Overclocked.Domain.CategoryAggregate;
using Overclocked.Domain.CategoryAggregate.ValueObjects;
using Overclocked.Domain.Common.Shared.ValueObjects.Image;

namespace Overclocked.Infrastructure.Persistence.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("categories");

        // Attributes
        builder.HasKey(c => c.Id);
        builder
            .Property(c => c.Id)
            .ValueGeneratedNever()
            .HasConversion(id => id.Value, value => CategoryId.Create(value))
            .IsRequired();

        builder.Property(c => c.Name).HasMaxLength(50).IsRequired();

        builder.Property(c => c.NormalizedName).HasMaxLength(50).IsRequired();

        builder.Property(c => c.Image).HasConversion(name => name.Value, value => Image.Load(value)).IsRequired();

        builder.Property(c => c.CreatedAt).HasColumnType("timestamptz").IsRequired();

        builder.Property(c => c.UpdatedAt).HasColumnType("timestamptz").IsRequired();

        // Indexes
        builder.HasIndex(c => c.NormalizedName).IsUnique();
    }
}
