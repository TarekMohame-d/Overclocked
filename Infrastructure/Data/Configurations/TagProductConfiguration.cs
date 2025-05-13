using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class TagProductConfiguration : IEntityTypeConfiguration<TagProduct>
    {
        public void Configure(EntityTypeBuilder<TagProduct> builder)
        {
            builder.HasKey(tp => new { tp.TagId, tp.ProductId });

            builder.HasOne(tp => tp.Tag)
                   .WithMany(t => t.TagProducts)
                   .HasForeignKey(tp => tp.TagId);

            builder.HasOne(tp => tp.Product)
                   .WithMany(p => p.TagProducts)
                   .HasForeignKey(tp => tp.ProductId);
        }
    }
}
