using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Overclocked.Domain.OrderAggregate.ValueObjects;
using Overclocked.Domain.PaymentAggregate;
using Overclocked.Domain.PaymentAggregate.ValueObjects;

namespace Overclocked.Infrastructure.Persistence.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable(
            "payments",
            t =>
            {
                IEnumerable<int> validIds = Enum.GetValues<PaymentStatus>().Cast<int>();
                t.HasCheckConstraint("CK_Payments_Status", $"status_id IN ({string.Join(", ", validIds)})");
            }
        );

        // Attributes
        builder.HasKey(p => p.Id);
        builder
            .Property(p => p.Id)
            .ValueGeneratedNever()
            .HasConversion(id => id.Value, value => PaymentId.Create(value))
            .IsRequired();

        builder.Property(p => p.OrderId).HasConversion(id => id.Value, value => OrderId.Create(value)).IsRequired();

        builder.Property(p => p.Status).HasColumnName("status_id").HasConversion<int>().IsRequired();

        builder.Property(p => p.PaymentProvider).HasMaxLength(50).IsRequired();

        builder.Property(p => p.PaymentMethod).HasMaxLength(50).IsRequired();

        builder.Property(p => p.TransactionId).IsRequired(false);

        builder.Property(p => p.CreatedAt).IsRequired();

        builder.Property(p => p.UpdatedAt).IsRequired();

        ConfigureAmount(builder);

        // Indexes
        builder.HasIndex(p => p.PaymentProvider);

        builder.HasIndex(p => p.PaymentMethod);

        builder.HasIndex(p => p.Status);

        builder.HasIndex(p => p.OrderId).IsUnique();
    }

    private static void ConfigureAmount(EntityTypeBuilder<Payment> builder) =>
        builder.ComplexProperty(
            p => p.Amount,
            moneyBuilder =>
            {
                moneyBuilder.Property(m => m.Value).HasColumnName("amount").HasColumnType("decimal(10,2)").IsRequired();

                moneyBuilder.Property(m => m.Currency).HasColumnName("currency").HasMaxLength(3).IsRequired();
            }
        );
}
