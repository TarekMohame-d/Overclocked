using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class SpecificationConfiguration : IEntityTypeConfiguration<Specification>
{
    public void Configure(EntityTypeBuilder<Specification> builder)
    {
        // Attributes
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id)
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(s => s.ProductId)
            .IsRequired();
        builder.Property(s => s.Name)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(s => s.NormalizedName)
            .HasMaxLength(50)
            .HasComputedColumnSql("UPPER(\"Name\")", stored: true);

        builder.Property(s => s.Value)
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(s => s.CreatedAt)
            .HasColumnType("timestamptz")
            .HasDefaultValueSql("NOW()");

        builder.Property(s => s.UpdatedAt)
            .HasColumnType("timestamptz")
            .HasDefaultValueSql("NOW()");

        // Relationships
        builder.HasOne(s => s.Product)
            .WithMany(p => p.Specifications)
            .HasForeignKey(s => s.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(s => s.ProductId);

        builder.HasIndex(s => new { s.Name, s.ProductId })
            .IsUnique();

        builder.HasIndex(s => new { s.NormalizedName, s.ProductId })
            .IsUnique();
    }
}
