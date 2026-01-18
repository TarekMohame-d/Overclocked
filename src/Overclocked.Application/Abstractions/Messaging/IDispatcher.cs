using Overclocked.SharedKernel;

namespace Overclocked.Application.Abstractions.Messaging;

public interface IDispatcher
{
    Task<Result<TResponse>> Send<TResponse>(IRequest<TResponse> request, CancellationToken ct = default);
    Task<Result> Send(IRequest request, CancellationToken ct = default);
}
