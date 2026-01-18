using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Overclocked.Infrastructure.Inbox;

public class PaymentWebhookConfiguration : IEntityTypeConfiguration<PaymentWebhook>
{
    public void Configure(EntityTypeBuilder<PaymentWebhook> builder)
    {
        builder.ToTable("payment_webhooks");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.RetryCount).HasDefaultValue(0).IsRequired();

        builder.Property(x => x.TransactionId).HasMaxLength(500);

        builder.Property(x => x.Payload);

        builder.Property(x => x.ErrorLog).HasMaxLength(4000);

        builder.Property(x => x.CreatedOnUtc).IsRequired();

        builder.Property(x => x.ProcessedOnUtc).IsRequired(false);

        builder.HasIndex(x => new { x.ProcessedOnUtc, x.CreatedOnUtc });
    }
}
