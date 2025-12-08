using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Overclocked.Application.Abstraction.Messaging;
using Overclocked.Domain.Common.Primitives;

namespace Overclocked.Infrastructure.Outbox;

public class DomainEventDispatcher(IServiceProvider serviceProvider) : IDomainEventDispatcher
{
    public async Task Dispatch(IDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        Type eventType = domainEvent.GetType();

        // Resolve all handlers for this event type
        Type handlerType = typeof(IDomainEventHandler<>).MakeGenericType(eventType);

        IEnumerable<object?> handlers = serviceProvider.GetServices(handlerType);

        foreach(var handler in handlers)
        {
            MethodInfo? method = handlerType.GetMethod("Handle");

            if(method is null)
            {
                continue;
            }

            var task = (Task)method.Invoke(handler, new object[] { domainEvent, cancellationToken })!;

            await task;
        }
    }
}
