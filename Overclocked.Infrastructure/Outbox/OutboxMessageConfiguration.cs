using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Overclocked.Infrastructure.Outbox;

internal sealed class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("outbox_messages");

        builder.HasKey(x => x.Id);


        builder.HasIndex(x => new { x.ProcessedOnUtc, x.OccurredOnUtc });

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.RetryCount)
            .HasDefaultValue(0)
            .IsRequired();

        builder.Property(x => x.Type)
            .HasMaxLength(500);

        builder.Property(x => x.Error)
            .HasMaxLength(4000);
    }
}
