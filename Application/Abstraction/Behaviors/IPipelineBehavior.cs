using Application.Abstraction.Messaging;

namespace Application.Abstraction.Behaviors;

public delegate Task<TResponse> RequestHandlerDelegate<TResponse>(CancellationToken cancellationToken);


public interface IPipelineBehavior<in TRequest, TResponse>
    where TRequest : IRequest
{
    Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken);
}
