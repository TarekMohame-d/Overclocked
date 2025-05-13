using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.ProductName).HasMaxLength(200).IsRequired();
            builder.Property(p => p.Description).HasMaxLength(1500).IsRequired();
            builder.Property(p => p.Price).HasColumnType("decimal(10,2)").IsRequired();
            builder.Property(p => p.StockQuantity).IsRequired();
            builder.Property(c => c.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");

            builder.HasOne(p => p.Category)
                .WithMany(cat => cat.Products)
                .HasForeignKey(p => p.CategoryId);

            builder.HasOne(p => p.Brand)
                .WithMany(b => b.Products)
                .HasForeignKey(p => p.BrandId);

            builder.HasMany(p => p.CartProducts)
                .WithOne(cp => cp.Product)
                .HasForeignKey(cp => cp.ProductId);

            builder.HasMany(p => p.Reviews)
                .WithOne(r => r.Product)
                .HasForeignKey(r => r.ProductId);

            builder.HasMany(p => p.OrderItems)
                .WithOne(oi => oi.Product)
                .HasForeignKey(oi => oi.ProductId);

            builder.HasMany(p => p.TagProducts)
                .WithOne(tp => tp.Product)
                .HasForeignKey(tp => tp.ProductId);

            builder.HasMany(p => p.ProductImages)
                .WithOne(pi => pi.Product)
                .HasForeignKey(pi => pi.ProductId);

            builder.HasMany(p => p.Specifications)
                .WithOne(s => s.Product)
                .HasForeignKey(s => s.ProductId);

            builder.HasMany(p => p.WishlistProducts)
                .WithOne(wlp => wlp.Product)
                .HasForeignKey(wlp => wlp.ProductId);

            // Indexes
            builder.HasIndex(p => p.ProductName)
                   .IsUnique();
        }
    }
}
