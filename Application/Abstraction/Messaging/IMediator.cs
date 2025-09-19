namespace Application.Abstraction.Messaging;

public interface IMediator
{
    Task<TResponse> Send<TResponse>(IQuery<TResponse> request, CancellationToken cancellationToken = default);
    Task<TResponse> Send<TResponse>(ICommand<TResponse> request, CancellationToken cancellationToken = default);
    Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
        where TNotification : INotification;
}
