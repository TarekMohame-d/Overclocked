using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class AddressConfiguration : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> builder)
    {
        // Attributes
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).ValueGeneratedNever().IsRequired();
        builder.Property(a => a.UserId).IsRequired();
        builder.Property(a => a.City).HasMaxLength(30).IsRequired();
        builder.Property(a => a.Street).HasMaxLength(100).IsRequired();
        builder.Property(a => a.Description).HasMaxLength(300).IsRequired();
        builder.Property(a => a.IsDeleted).HasDefaultValue(false);
        builder.Property(a => a.CreatedAt).HasColumnType("timestamptz")
            .HasDefaultValueSql("NOW()");
        builder.Property(a => a.UpdatedAt).HasColumnType("timestamptz")
            .HasDefaultValueSql("NOW()");

        // Relationships
        builder.HasOne(a => a.User)
            .WithMany(u => u.Addresses)
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(a => a.UserId);
        builder.HasIndex(a => a.City);
    }
}
