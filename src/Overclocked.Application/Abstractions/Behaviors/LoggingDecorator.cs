using Microsoft.Extensions.Logging;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.SharedKernel;
using Serilog.Context;

namespace Overclocked.Application.Abstractions.Behaviors;

internal static class LoggingDecorator
{
    internal sealed class RequestHandler<TRequest, TResponse>(
        IRequestHandler<TRequest, TResponse> innerHandler,
        ILogger<RequestHandler<TRequest, TResponse>> logger
    ) : IRequestHandler<TRequest, TResponse>, IDecorator
        where TRequest : IRequest<TResponse>
    {
        public async Task<Result<TResponse>> Handle(TRequest request, CancellationToken ct)
        {
            var requestName = typeof(TRequest).Name;

            logger.LogInformation("Processing request {Request}", requestName);

            Result<TResponse> result = await innerHandler.Handle(request, ct);

            if (result.IsSuccess)
            {
                logger.LogInformation("Completed request {Request}", requestName);
            }
            else
            {
                using (LogContext.PushProperty("Error", result.Error, true))
                {
                    logger.LogError("Completed request {Request} with error", requestName);
                }
            }

            return result;
        }
    }

    internal sealed class RequestHandler<TRequest>(
        IRequestHandler<TRequest> innerHandler,
        ILogger<RequestHandler<TRequest>> logger
    ) : IRequestHandler<TRequest>, IDecorator
        where TRequest : IRequest
    {
        public async Task<Result> Handle(TRequest request, CancellationToken ct)
        {
            var requestName = typeof(TRequest).Name;

            logger.LogInformation("Processing request {Request}", requestName);

            Result result = await innerHandler.Handle(request, ct);

            if (result.IsSuccess)
            {
                logger.LogInformation("Completed request {Request}", requestName);
            }
            else
            {
                using (LogContext.PushProperty("Error", result.Error, true))
                {
                    logger.LogError("Completed request {Request} with error", requestName);
                }
            }

            return result;
        }
    }
}
