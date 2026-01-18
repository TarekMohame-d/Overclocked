using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Newtonsoft.Json;
using Overclocked.SharedKernel.Primitives;

namespace Overclocked.Infrastructure.Outbox;

public sealed class InsertOutboxMessagesInterceptor : SaveChangesInterceptor
{
    private static readonly JsonSerializerSettings _serializerSettings = new() { TypeNameHandling = TypeNameHandling.All };

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default
    )
    {
        if (eventData.Context is not null)
            InsertOutboxMessages(eventData.Context);

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private static void InsertOutboxMessages(DbContext dbContext)
    {
        var utcNow = DateTimeOffset.UtcNow;

        var outboxMessages = dbContext
            .ChangeTracker.Entries()
            .Where(e => e.Entity is IAggregateRoot)
            .Select(e => (IAggregateRoot)e.Entity)
            .SelectMany(aggregate =>
            {
                var domainEvents = aggregate.DomainEvents.ToList();
                aggregate.ClearDomainEvents();
                return domainEvents;
            })
            .Select(domainEvent => new OutboxMessage(
                Guid.CreateVersion7(),
                domainEvent.GetType().Name,
                JsonConvert.SerializeObject(domainEvent, _serializerSettings),
                utcNow
            ))
            .ToList();

        dbContext.Set<OutboxMessage>().AddRange(outboxMessages);
    }
}
