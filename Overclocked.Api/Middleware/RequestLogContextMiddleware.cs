using Microsoft.Extensions.Primitives;
using Serilog.Context;

namespace Overclocked.Api.Middleware;

public class RequestLogContextMiddleware(RequestDelegate next)
{
    private const string CorrelationIdHeaderName = "Correlation-Id";

    public Task Invoke(HttpContext context)
    {
        using(LogContext.PushProperty("CorrelationId", GetCorrelationId(context)))
        {
            return next.Invoke(context);
        }
    }

    private static string GetCorrelationId(HttpContext context)
    {
        context.Request.Headers.TryGetValue(CorrelationIdHeaderName, out StringValues correlationId);

        return correlationId.FirstOrDefault() ?? context.TraceIdentifier;
    }
}
