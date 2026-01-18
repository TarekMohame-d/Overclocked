namespace Overclocked.Infrastructure.Outbox;

public interface IProcessOutboxMessagesJob
{
    Task ProcessOutboxMessagesAsync();
}
