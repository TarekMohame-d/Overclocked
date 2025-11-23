using Microsoft.Extensions.DependencyInjection;

namespace Application.Abstraction.Messaging;

public class EventDispatcher(IServiceProvider serviceProvider) : IEventDispatcher
{
    public async Task DispatchAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        // Get the generic handler type (e.g., IEventHandler<UserRegisteredEvent>)
        Type handlerType = typeof(IEventHandler<>).MakeGenericType(domainEvent.GetType());

        // Resolve all handlers from DI
        IEnumerable<object?> handlers = serviceProvider.GetServices(handlerType);

        foreach(var handler in handlers)
        {
            if(handler is null)
                continue;

            await ((IEventHandler)handler).Handle(domainEvent, cancellationToken);
        }
    }
}
