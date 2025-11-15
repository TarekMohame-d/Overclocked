using Microsoft.Extensions.DependencyInjection;

namespace Application.Abstraction.Messaging;

public class EventDispatcher : IEventDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public EventDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task DispatchAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        // Get the generic handler type (e.g., IEventHandler<UserRegisteredEvent>)
        var handlerType = typeof(IEventHandler<>).MakeGenericType(domainEvent.GetType());

        // Resolve all handlers from DI
        var handlers = _serviceProvider.GetServices(handlerType);

        foreach (var handler in handlers)
        {
            if (handler is null) continue;

            await ((IEventHandler)handler).Handle(domainEvent, cancellationToken);
        }
    }
}
