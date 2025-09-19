using Application.Abstraction.Behaviors;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Abstraction.Messaging;

public class Mediator : IMediator
{
    private readonly IServiceProvider _serviceProvider;

    public Mediator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    // Send - For command/query operations
    public async Task<TResponse> Send<TResponse>(IQuery<TResponse> request, CancellationToken cancellationToken = default)
    {
        var requestType = request.GetType();
        var handlerType = typeof(IQueryHandler<,>).MakeGenericType(requestType, typeof(TResponse));

        var handler = _serviceProvider.GetService(handlerType);
        if (handler is null)
            throw new InvalidOperationException($"No handler registered for {requestType.Name}");

        // Resolve pipeline behaviors for the actual request type
        var behaviorType = typeof(IPipelineBehavior<,>).MakeGenericType(requestType, typeof(TResponse));
        var behaviors = _serviceProvider.GetServices(behaviorType).Cast<dynamic>().ToList();

        // Build the handler delegate
        RequestHandlerDelegate<TResponse> handlerDelegate = (cancellationToken) =>
            ((dynamic)handler).Handle((dynamic)request, cancellationToken);

        // Wrap with pipeline behaviors in reverse order
        RequestHandlerDelegate<TResponse> pipeline = handlerDelegate;
        foreach (var behavior in behaviors.Reverse<dynamic>())
        {
            var current = pipeline;
            pipeline = (cancellationToken) => behavior.Handle((dynamic)request, current, cancellationToken);
        }

        return await pipeline(cancellationToken);
    }

    public async Task<TResponse> Send<TResponse>(ICommand<TResponse> request, CancellationToken cancellationToken = default)
    {
        var requestType = request.GetType();
        var handlerType = typeof(ICommandHandler<,>).MakeGenericType(requestType, typeof(TResponse));

        var handler = _serviceProvider.GetService(handlerType);
        if (handler is null)
            throw new InvalidOperationException($"No handler registered for {requestType.Name}");

        var behaviorType = typeof(IPipelineBehavior<,>).MakeGenericType(requestType, typeof(TResponse));
        var behaviors = _serviceProvider.GetServices(behaviorType).Cast<dynamic>().ToList();

        RequestHandlerDelegate<TResponse> handlerDelegate = (cancellationToken) =>
            ((dynamic)handler).Handle((dynamic)request, cancellationToken);

        RequestHandlerDelegate<TResponse> pipeline = handlerDelegate;
        foreach (var behavior in behaviors.Reverse<dynamic>())
        {
            var current = pipeline;
            pipeline = (cancellationToken) => behavior.Handle((dynamic)request, current, cancellationToken);
        }

        return await pipeline(cancellationToken);
    }

    // Publish - For notification operations
    public async Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
        where TNotification : INotification
    {
        var handlerType = typeof(INotificationHandler<>).MakeGenericType(notification.GetType());
        var handlers = _serviceProvider.GetServices(handlerType);

        var tasks = handlers.Select(handler =>
        {
            return (Task)((dynamic)handler!).Handle((dynamic)notification, cancellationToken);
        });

        await Task.WhenAll(tasks);
    }
}
