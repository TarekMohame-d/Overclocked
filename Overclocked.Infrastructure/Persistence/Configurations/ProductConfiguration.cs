using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Overclocked.Domain.BrandAggregate;
using Overclocked.Domain.BrandAggregate.ValueObjects;
using Overclocked.Domain.CategoryAggregate;
using Overclocked.Domain.CategoryAggregate.ValueObjects;
using Overclocked.Domain.ProductAggregate;
using Overclocked.Domain.ProductAggregate.ValueObjects;
using Overclocked.Domain.TagAggregate;
using Overclocked.Domain.TagAggregate.ValueObjects;

namespace Overclocked.Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        // Attributes
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id)
            .ValueGeneratedNever()
            .HasConversion(
                id => id.Value,
                value => ProductId.Create(value))
            .IsRequired();

        builder.Property(p => p.CategoryId)
            .HasConversion(
                id => id.Value,
                value => CategoryId.Create(value))
            .IsRequired();

        builder.Property(p => p.BrandId)
            .HasConversion(
                id => id.Value,
                value => BrandId.Create(value))
            .IsRequired();

        builder.Property(p => p.Name)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(p => p.NormalizedName)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(p => p.Thumbnail)
            .IsRequired();

        builder.Property(p => p.Description)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(p => p.RowVersion).IsRowVersion();

        builder.ComplexProperty(p => p.Price, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("Price")
                .HasColumnType("decimal(8,2)")
                .IsRequired();

            money.Property(m => m.Currency)
                .HasColumnName("Currency")
                .HasColumnType("varchar(5)")
                .IsRequired();
        });

        builder.ComplexProperty(p => p.Discount, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("Discount")
                .HasColumnType("decimal(2,2)")
                .IsRequired();

            money.Ignore(m => m.Currency); // Not stored
        });

        builder.OwnsOne(p => p.ProductRating, rating =>
        {
            rating.Property(r => r.TotalScore)
                .HasColumnName("TotalScore")
                .IsRequired();

            rating.Property(r => r.ReviewCount)
                .HasColumnName("ReviewCount")
                .IsRequired();

            rating.Ignore(r => r.AverageRating);

            rating.HasIndex(r => r.TotalScore);
            rating.HasIndex(r => r.ReviewCount);
        });

        builder.Property(p => p.StockQuantity)
            .IsRequired();

        builder.Property(p => p.IsDeleted)
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(c => c.CreatedAt)
            .HasColumnType("timestamptz")
            .IsRequired();

        builder.Property(c => c.UpdatedAt)
            .HasColumnType("timestamptz")
            .IsRequired();

        // Relationships
        builder.HasOne(p => p.Brand)
            .WithMany()
            .HasForeignKey(p => p.BrandId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Category)
            .WithMany()
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.OwnsMany(p => p.Images, pi =>
        {
            pi.ToTable("ProductImages");

            pi.WithOwner().HasForeignKey("ProductId"); // shadow property

            pi.HasKey(pi => pi.Id);
            pi.Property(pi => pi.Id)
                .ValueGeneratedNever()
                .HasConversion(
                    id => id.Value,
                    value => ProductImageId.Create(value))
                .IsRequired();

            pi.Property(pi => pi.ImageUrl)
                .IsRequired();

            pi.Property(pi => pi.CreatedAt)
                .HasColumnType("timestamptz")
                .IsRequired();
        });

        builder.OwnsMany(p => p.Specifications, ps =>
        {
            ps.ToTable("ProductSpecifications");

            ps.WithOwner().HasForeignKey("ProductId"); // shadow property

            ps.HasKey(s => s.Id);
            ps.Property(s => s.Id)
                .ValueGeneratedNever()
                .HasConversion(
                    id => id.Value,
                    value => SpecificationId.Create(value))
                .IsRequired();

            ps.Property(s => s.Name)
                .HasMaxLength(50)
                .IsRequired();

            ps.Property(s => s.NormalizedName)
                .HasMaxLength(50)
                .IsRequired();

            ps.Property(s => s.Value)
                .HasMaxLength(300)
                .IsRequired();

            ps.Property(s => s.CreatedAt)
                .HasColumnType("timestamptz")
                .IsRequired();

            ps.Property(s => s.UpdatedAt)
                .HasColumnType("timestamptz")
                .IsRequired();
        });

        builder.OwnsMany(p => p.Tags, pt =>
        {
            pt.ToTable("ProductTags");

            pt.WithOwner().HasForeignKey("ProductId");

            pt.Property(t => t.TagId)
                .HasColumnName("TagId")
                .HasConversion(
                    id => id.Value,
                    value => TagId.Create(value))
                .IsRequired();

            pt.HasKey("ProductId", "TagId");

            pt.HasOne(t => t.Tag)
                .WithMany()
                .HasForeignKey(t => t.TagId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Navigation(p => p.Images)
            .AutoInclude(false)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Navigation(p => p.Specifications)
            .AutoInclude(false)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Navigation(p => p.Tags)
            .AutoInclude(false)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Navigation(p => p.Brand)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Navigation(p => p.Category)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        // Indexes
        builder.HasIndex(p => p.Name)
            .IsUnique();

        builder.HasIndex(p => p.NormalizedName)
            .IsUnique();

        builder.HasIndex(p => p.BrandId);

        builder.HasIndex(p => p.CategoryId);
    }
}
