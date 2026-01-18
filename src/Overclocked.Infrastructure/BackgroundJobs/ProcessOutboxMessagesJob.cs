using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Infrastructure.Outbox;
using Overclocked.Infrastructure.Persistence;
using Overclocked.SharedKernel.Primitives;

namespace Overclocked.Infrastructure.BackgroundJobs;

public sealed class ProcessOutboxMessagesJob(IServiceScopeFactory scopeFactory, ILogger<ProcessOutboxMessagesJob> logger)
    : IProcessOutboxMessagesJob
{
    private const int BatchSize = 15;
    private const int MaxRetries = 3;

    private static readonly JsonSerializerSettings _serializerSettings = new() { TypeNameHandling = TypeNameHandling.All };

    [DisableConcurrentExecution(timeoutInSeconds: 0)]
    [AutomaticRetry(Attempts = 3, OnAttemptsExceeded = AttemptsExceededAction.Fail)]
    public async Task ProcessOutboxMessagesAsync()
    {
        logger.LogInformation("Beginning to process outbox messages");
        await using AsyncServiceScope scope = scopeFactory.CreateAsyncScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        List<OutboxMessage> messages = await dbContext
            .Set<OutboxMessage>()
            .AsTracking()
            .Where(m => m.ProcessedOnUtc == null && m.RetryCount < MaxRetries)
            .OrderBy(m => m.OccurredOnUtc)
            .Take(BatchSize)
            .ToListAsync();

        if (messages.Count == 0)
            return;

        foreach (OutboxMessage message in messages)
        {
            try
            {
                IDomainEvent? domainEvent = JsonConvert.DeserializeObject<IDomainEvent>(message.Payload, _serializerSettings);

                if (domainEvent is null)
                {
                    message.MarkProcessed(); // Invalid JSON, mark done to skip
                    continue;
                }

                await using (AsyncServiceScope handlerScope = scopeFactory.CreateAsyncScope())
                {
                    IDomainEventDispatcher dispatcher = handlerScope.ServiceProvider.GetRequiredService<IDomainEventDispatcher>();
                    await dispatcher.Dispatch(domainEvent);
                }

                message.MarkProcessed();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing outbox message {Id}", message.Id);
                message.HandleFailure(ex.ToString(), MaxRetries);
            }
        }

        await dbContext.SaveChangesAsync();
    }
}
