using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class TagConfiguration : IEntityTypeConfiguration<Tag>
    {
        public void Configure(EntityTypeBuilder<Tag> builder)
        {
            builder.HasKey(t => t.Id);
            builder.Property(t => t.TagName).HasMaxLength(200).IsRequired();
            builder.Property(t => t.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");

            builder.HasMany(t => t.TagProducts)
                   .WithOne(tp => tp.Tag)
                   .HasForeignKey(tp => tp.TagId);

            // Indexes
            builder.HasIndex(t => t.TagName).IsUnique();
        }
    }
}
