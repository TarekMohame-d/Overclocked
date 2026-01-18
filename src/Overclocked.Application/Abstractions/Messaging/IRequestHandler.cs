using Overclocked.SharedKernel;

namespace Overclocked.Application.Abstractions.Messaging;

public interface IRequestHandler<in TRequest>
    where TRequest : IRequest
{
    Task<Result> Handle(TRequest request, CancellationToken ct);
}

public interface IRequestHandler<in TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    Task<Result<TResponse>> Handle(TRequest request, CancellationToken ct);
}
