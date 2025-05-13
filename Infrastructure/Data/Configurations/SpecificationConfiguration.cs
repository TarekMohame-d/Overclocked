using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class SpecificationConfiguration : IEntityTypeConfiguration<Specification>
    {
        public void Configure(EntityTypeBuilder<Specification> builder)
        {
            builder.HasKey(s => s.Id);
            builder.Property(s => s.Name).HasMaxLength(200).IsRequired();
            builder.Property(s => s.Value).HasMaxLength(500).IsRequired();
            builder.Property(s => s.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");

            builder.HasOne(s => s.Product)
                   .WithMany(p => p.Specifications)
                   .HasForeignKey(s => s.ProductId);

            // Indexes
            builder.HasIndex(s => s.Name).IsUnique();
        }
    }
}
