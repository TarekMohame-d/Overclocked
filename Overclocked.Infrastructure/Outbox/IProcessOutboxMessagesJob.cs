namespace Overclocked.Infrastructure.Outbox;

public interface IProcessOutboxMessagesJob
{
    Task ProcessOutboxMessages();
}
