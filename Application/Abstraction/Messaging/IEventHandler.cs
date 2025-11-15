namespace Application.Abstraction.Messaging;

public interface IEventHandler
{
    Task Handle(IDomainEvent domainEvent, CancellationToken cancellationToken);
}

public interface IEventHandler<in TEvent> : IEventHandler where TEvent : IDomainEvent
{
    Task HandleAsync(TEvent domainEvent, CancellationToken cancellationToken);
}
