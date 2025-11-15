namespace Application.Abstraction.Messaging;

public interface IEventDispatcher
{
    Task DispatchAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default);
}
