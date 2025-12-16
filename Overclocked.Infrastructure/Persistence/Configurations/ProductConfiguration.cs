using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Overclocked.Domain.BrandAggregate.ValueObjects;
using Overclocked.Domain.CategoryAggregate.ValueObjects;
using Overclocked.Domain.ProductAggregate;
using Overclocked.Domain.ProductAggregate.ValueObjects;
using Overclocked.Domain.TagAggregate.ValueObjects;

namespace Overclocked.Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("products");

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

        ConfigureProductPrice(builder);

        ConfigureProductDiscount(builder);

        ConfigureProductRating(builder);

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

        ConfigureProductImages(builder);

        ConfigureProductSpecifications(builder);

        ConfigureProductTags(builder);

        builder.Navigation(p => p.Images)
            .AutoInclude(false)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Navigation(p => p.Specifications)
            .AutoInclude(false)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Navigation(p => p.ProductTags)
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

    private static void ConfigureProductPrice(EntityTypeBuilder<Product> builder)
    {
        builder.ComplexProperty(p => p.Price, moneyBuilder =>
        {
            moneyBuilder.Property(m => m.Amount)
                .HasColumnName("price")
                .HasColumnType("decimal(8,2)")
                .IsRequired();

            moneyBuilder.Property(m => m.Currency)
                .HasColumnName("currency")
                .HasMaxLength(3)
                .IsRequired();
        });
    }

    private static void ConfigureProductDiscount(EntityTypeBuilder<Product> builder)
    {
        builder.ComplexProperty(p => p.Discount, moneyBuilder =>
        {
            moneyBuilder.Property(m => m.Amount)
                .HasColumnName("discount")
                .HasColumnType("decimal(2,2)")
                .IsRequired();

            moneyBuilder.Ignore(m => m.Currency); // Not stored
        });
    }

    private static void ConfigureProductRating(EntityTypeBuilder<Product> builder)
    {
        builder.OwnsOne(p => p.ProductRating, prBuilder =>
        {
            prBuilder.Property(r => r.TotalScore)
                .HasColumnName("total_score")
                .IsRequired();

            prBuilder.Property(r => r.ReviewCount)
                .HasColumnName("review_count")
                .IsRequired();

            prBuilder.Ignore(r => r.AverageRating);

            prBuilder.HasIndex(r => r.TotalScore);
            prBuilder.HasIndex(r => r.ReviewCount);
        });
    }

    private static void ConfigureProductImages(EntityTypeBuilder<Product> builder)
    {
        builder.OwnsMany(p => p.Images, pi =>
        {
            pi.ToTable("product_images");

            pi.WithOwner(); // shadow property

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
    }

    private static void ConfigureProductSpecifications(EntityTypeBuilder<Product> builder)
    {
        builder.OwnsMany(p => p.Specifications, ps =>
        {
            ps.ToTable("product_specifications");

            ps.WithOwner(); // shadow property

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
    }

    private static void ConfigureProductTags(EntityTypeBuilder<Product> builder)
    {
        builder.OwnsMany(p => p.ProductTags, pt =>
        {
            pt.ToTable("product_tags");

            pt.WithOwner().HasForeignKey("ProductId");

            pt.Property<ProductId>("ProductId")
                .HasColumnName("product_id");

            pt.Property(t => t.TagId)
                .HasColumnName("tag_id")
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
    }
}
