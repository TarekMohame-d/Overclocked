using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Overclocked.Domain.ProductAggregate;
using Overclocked.Domain.ProductAggregate.ValueObjects;
using Overclocked.Domain.ReviewAggregate;
using Overclocked.Domain.ReviewAggregate.ValueObjects;
using Overclocked.Domain.UserAggregate;
using Overclocked.Domain.UserAggregate.ValueObjects;

namespace Overclocked.Infrastructure.Persistence.Configurations;

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.ToTable("reviews");

        builder.HasKey(r => r.Id);
        builder
            .Property(r => r.Id)
            .ValueGeneratedNever()
            .HasConversion(id => id.Value, value => ReviewId.Create(value))
            .IsRequired();

        builder.Property(r => r.UserId).HasConversion(id => id.Value, value => UserId.Create(value)).IsRequired();

        builder.Property(r => r.ProductId).HasConversion(id => id.Value, value => ProductId.Create(value)).IsRequired();

        builder.Property(r => r.Comment).HasMaxLength(500).IsRequired();

        builder.Property(r => r.Rating).IsRequired();

        builder.Property(r => r.CreatedAt).HasColumnType("timestamptz").IsRequired();

        builder.Property(r => r.UpdatedAt).HasColumnType("timestamptz").IsRequired();

        // Relationships
        builder.HasOne(r => r.User).WithMany().HasForeignKey(r => r.UserId).OnDelete(DeleteBehavior.SetNull);

        builder.HasOne<Product>().WithMany().HasForeignKey(r => r.ProductId).OnDelete(DeleteBehavior.Cascade);

        ConfigureReviewReply(builder);

        builder.Navigation(r => r.ReviewReply).UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Navigation(r => r.User).UsePropertyAccessMode(PropertyAccessMode.Field);

        // Indexes
        builder.HasIndex(r => r.UserId);
        builder.HasIndex(r => r.ProductId);
        builder.HasIndex(r => r.Rating);
    }

    private static void ConfigureReviewReply(EntityTypeBuilder<Review> builder) =>
        builder.OwnsOne(
            r => r.ReviewReply,
            reviewReplyBuilder =>
            {
                reviewReplyBuilder.ToTable("review_replies");

                reviewReplyBuilder.WithOwner().HasForeignKey("ReviewId"); // shadow property

                reviewReplyBuilder.Property<ReviewId>("ReviewId").HasColumnName("review_id");

                reviewReplyBuilder.HasKey(rr => rr.Id);
                reviewReplyBuilder
                    .Property(rr => rr.Id)
                    .ValueGeneratedNever()
                    .HasConversion(id => id.Value, value => ReviewReplyId.Create(value))
                    .IsRequired();

                reviewReplyBuilder
                    .Property(rr => rr.EmployeeId)
                    .HasConversion(id => id.Value, value => UserId.Create(value))
                    .IsRequired();

                reviewReplyBuilder.Property(rr => rr.Reply).HasMaxLength(500).IsRequired();

                reviewReplyBuilder.Property(rr => rr.CreatedAt).HasColumnType("timestamptz").IsRequired();

                reviewReplyBuilder.Property(rr => rr.UpdatedAt).HasColumnType("timestamptz").IsRequired();

                reviewReplyBuilder.HasOne<User>().WithMany().HasForeignKey(rr => rr.EmployeeId).OnDelete(DeleteBehavior.SetNull);
            }
        );
}
