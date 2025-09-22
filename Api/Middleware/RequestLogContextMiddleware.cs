using Microsoft.Extensions.Primitives;
using Serilog.Context;

namespace Api.Middleware;

public class RequestLogContextMiddleware
{
    private readonly RequestDelegate _next;
    private const string CorrelationIdHeaderName = "Correlation-Id";
    public RequestLogContextMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public Task Invoke(HttpContext context)
    {
        using (LogContext.PushProperty("CorrelationId", GetCorrelationId(context)))
        {
            return _next.Invoke(context);
        }
    }

    private static string GetCorrelationId(HttpContext context)
    {
        context.Request.Headers.TryGetValue(
            CorrelationIdHeaderName,
            out StringValues correlationId);

        return correlationId.FirstOrDefault() ?? context.TraceIdentifier;
    }
}
