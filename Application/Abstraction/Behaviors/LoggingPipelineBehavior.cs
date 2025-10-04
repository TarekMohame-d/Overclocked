using Application.Abstraction.Messaging;
using Application.Common.Results;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace Application.Abstraction.Behaviors;

public sealed class LoggingPipelineBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest
    where TResponse : Result
{
    private readonly ILogger<LoggingPipelineBehavior<TRequest, TResponse>> _logger;

    public LoggingPipelineBehavior(ILogger<LoggingPipelineBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        string requestName = request.GetType().Name;

        _logger.LogInformation("Processing request {RequestName}", requestName);

        TResponse result = await next(cancellationToken);

        if (result.IsSuccess)
        {
            _logger.LogInformation("Completed request {RequestName}", requestName);
        }
        else
        {
            using (LogContext.PushProperty("Errors", result.Error, true))
            {
                _logger.LogError("Completed request {@RequestName} with errors", requestName);
            }
        }

        return result;
    }
}
