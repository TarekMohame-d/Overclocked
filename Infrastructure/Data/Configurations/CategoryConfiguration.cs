using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        // Attributes
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).ValueGeneratedNever().IsRequired();
        builder.Property(c => c.Name).HasMaxLength(50).IsRequired();
        builder.Property(c => c.Image).IsRequired();
        builder.Property(c => c.CreatedAt).HasColumnType("timestamptz")
            .HasDefaultValueSql("NOW()");
        builder.Property(c => c.UpdatedAt).HasColumnType("timestamptz")
            .HasDefaultValueSql("NOW()");

        // Relationships

        // Indexes
        builder.HasIndex(c => c.Name)
            .IsUnique();
    }
}
