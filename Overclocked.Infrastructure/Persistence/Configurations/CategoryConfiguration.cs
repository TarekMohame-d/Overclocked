using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Overclocked.Domain.CategoryAggregate;
using Overclocked.Domain.CategoryAggregate.ValueObjects;

namespace Overclocked.Infrastructure.Persistence.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("categories");

        // Attributes
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id)
            .ValueGeneratedNever()
            .HasConversion(
                id => id.Value,
                value => CategoryId.Create(value))
            .IsRequired();

        builder.Property(c => c.Name)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(c => c.NormalizedName)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(c => c.ImageUrl)
            .IsRequired();

        builder.Property(c => c.CreatedAt)
            .HasColumnType("timestamptz")
            .IsRequired();

        builder.Property(c => c.UpdatedAt)
            .HasColumnType("timestamptz")
            .IsRequired();

        // Relationships

        // Indexes
        builder.HasIndex(c => c.Name)
            .IsUnique();

        builder.HasIndex(c => c.NormalizedName)
            .IsUnique();
    }
}
