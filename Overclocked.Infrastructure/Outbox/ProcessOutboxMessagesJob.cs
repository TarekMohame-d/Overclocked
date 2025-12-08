using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Overclocked.Application.Abstraction.Messaging;
using Overclocked.Domain.Common.Primitives;
using Overclocked.Infrastructure.Persistence;

namespace Overclocked.Infrastructure.Outbox;

public sealed class ProcessOutboxMessagesJob(
    ApplicationDbContext dbContext,
    IDomainEventDispatcher dispatcher,
    ILogger<ProcessOutboxMessagesJob> logger) : IProcessOutboxMessagesJob
{
    private const int BatchSize = 15;
    private static readonly JsonSerializerSettings _serializerSettings = new()
    {
        TypeNameHandling = TypeNameHandling.All
    };

    public async Task ProcessOutboxMessages()
    {
        logger.LogInformation("Beginning to process outbox messages");

        List<OutboxMessage> messages = await dbContext.Set<OutboxMessage>()
            .AsTracking()
            .Where(m => m.ProcessedOnUtc == null)
            .OrderBy(m => m.OccurredOnUtc)
            .Take(BatchSize)
            .ToListAsync();

        foreach(OutboxMessage message in messages)
        {
            try
            {
                IDomainEvent? domainEvent =
                    JsonConvert.DeserializeObject<IDomainEvent>(message.Payload, _serializerSettings);

                if(domainEvent is not null)
                {
                    await dispatcher.Dispatch(domainEvent);
                }

                message.MarkProcessed();
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "Exception occurred while processing outbox message {MessageId}", message.Id);
                message.MarkFailed(ex.ToString());
            }
        }

        await dbContext.SaveChangesAsync();
    }
}
