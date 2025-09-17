using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        // Attributes
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedNever().IsRequired();
        builder.Property(p => p.CategoryId).IsRequired();
        builder.Property(p => p.BrandId).IsRequired();
        builder.Property(p => p.Name).HasMaxLength(50).IsRequired();
        builder.Property(p => p.Thumbnail).IsRequired();
        builder.Property(p => p.Description).HasMaxLength(500).IsRequired();
        builder.Property(p => p.Price).HasColumnType("decimal(8,2)").IsRequired();
        builder.Property(p => p.Discount).HasColumnType("decimal(2,2)").IsRequired();
        builder.Property(p => p.Rating).HasColumnType("decimal(2,1)").IsRequired();
        builder.Property(p => p.StockQuantity).IsRequired();
        builder.Property(p => p.IsDeleted).HasDefaultValue(false);
        builder.Property(c => c.CreatedAt).HasColumnType("timestamptz")
            .HasDefaultValueSql("NOW()");
        builder.Property(c => c.UpdatedAt).HasColumnType("timestamptz")
            .HasDefaultValueSql("NOW()");

        // Relationships
        builder.HasOne(p => p.Category)
            .WithMany(cat => cat.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Brand)
            .WithMany(b => b.Products)
            .HasForeignKey(p => p.BrandId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(p => p.Name)
            .IsUnique();
        builder.HasIndex(p => p.Price);
        builder.HasIndex(p => p.Rating);
        builder.HasIndex(p => p.Discount);
        builder.HasIndex(p => p.StockQuantity);
        builder.HasIndex(p => p.BrandId);
        builder.HasIndex(p => p.CategoryId);
    }
}
