using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class TagProductConfiguration : IEntityTypeConfiguration<TagProduct>
{
    public void Configure(EntityTypeBuilder<TagProduct> builder)
    {
        // Attributes
        builder.HasKey(tp => new { tp.TagId, tp.ProductId });

        builder.Property(tp => tp.TagId)
            .IsRequired();
        builder.Property(tp => tp.ProductId)
            .IsRequired();

        // Relationships
        builder.HasOne(tp => tp.Tag)
            .WithMany(t => t.TagProducts)
            .HasForeignKey(tp => tp.TagId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(tp => tp.Product)
            .WithMany(p => p.TagProducts)
            .HasForeignKey(tp => tp.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(tp => tp.TagId);

        builder.HasIndex(tp => tp.ProductId);
    }
}
