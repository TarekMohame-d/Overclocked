using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.SharedKernel;

namespace Overclocked.Infrastructure;

public class NetCoreDispatcher(IServiceProvider serviceProvider) : IDispatcher
{
    public async Task<Result<TResponse>> Send<TResponse>(IRequest<TResponse> request, CancellationToken ct = default)
    {
        Type requestType = request.GetType();

        Type handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, typeof(TResponse));

        var handler = serviceProvider.GetRequiredService(handlerType);

        MethodInfo method = handlerType.GetMethod("Handle")!;

        var resultTask = (Task<Result<TResponse>>)method.Invoke(handler, [request, ct])!;

        return await resultTask;
    }

    public async Task<Result> Send(IRequest request, CancellationToken ct = default)
    {
        Type requestType = request.GetType();
        Type handlerType = typeof(IRequestHandler<>).MakeGenericType(requestType);

        var handler = serviceProvider.GetRequiredService(handlerType);

        MethodInfo method = handlerType.GetMethod("Handle")!;

        var resultTask = (Task<Result>)method.Invoke(handler, [request, ct])!;

        return await resultTask;
    }
}
