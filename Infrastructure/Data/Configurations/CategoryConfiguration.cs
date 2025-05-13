using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(c => c.Id);
            builder.Property(c => c.CategoryName).HasMaxLength(200).IsRequired();
            builder.Property(c => c.CategoryImage).HasMaxLength(500).IsRequired(false);
            builder.Property(c => c.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");

            builder.HasMany(c => c.Products)
                   .WithOne(p => p.Category)
                   .HasForeignKey(p => p.CategoryId);

            // Indexes
            builder.HasIndex(c => c.CategoryName).IsUnique();
        }
    }
}
