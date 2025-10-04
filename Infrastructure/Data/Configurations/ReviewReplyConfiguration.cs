using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class ReviewReplyConfiguration : IEntityTypeConfiguration<ReviewReply>
{
    public void Configure(EntityTypeBuilder<ReviewReply> builder)
    {
        // Attributes
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedNever().IsRequired();
        builder.Property(r => r.ReviewId).IsRequired();
        builder.Property(r => r.EmployeeId).IsRequired();
        builder.Property(r => r.Reply).HasMaxLength(500).IsRequired();
        builder.Property(r => r.CreatedAt).HasColumnType("timestamptz")
            .HasDefaultValueSql("NOW()");
        builder.Property(r => r.UpdatedAt).HasColumnType("timestamptz")
            .HasDefaultValueSql("NOW()");

        // Relationships
        builder.HasOne(rr => rr.Review)
            .WithOne(r => r.ReviewReply)
            .HasForeignKey<ReviewReply>(rr => rr.ReviewId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(rr => rr.Employee)
            .WithMany(u => u.ReviewReplies)
            .HasForeignKey(rr => rr.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(r => r.EmployeeId);
        builder.HasIndex(r => r.ReviewId);
    }
}
