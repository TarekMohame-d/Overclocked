using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.SharedKernel.Primitives;

namespace Overclocked.Infrastructure.Outbox;

public class DomainEventDispatcher(IServiceProvider serviceProvider) : IDomainEventDispatcher
{
    public async Task Dispatch(IDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        Type eventType = domainEvent.GetType();

        // Resolve all handlers for this event type
        Type handlerType = typeof(IDomainEventHandler<>).MakeGenericType(eventType);

        foreach (var handler in serviceProvider.GetServices(handlerType))
        {
            MethodInfo? method = handlerType.GetMethod("Handle");

            if (method is null)
                continue;

            var task = (Task)method.Invoke(handler, [domainEvent, cancellationToken])!;

            await task;
        }
    }
}
