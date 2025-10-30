using System.Net;
using Api.Extensions;
using Application.Common.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;

namespace Api.Middleware;

public class QueryParameterValidationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<QueryParameterValidationMiddleware> _logger;

    public QueryParameterValidationMiddleware(
        RequestDelegate next,
        ILogger<QueryParameterValidationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Check for duplicate query parameters
        var duplicateParams = context.Request.Query
            .Where(q => q.Value.Count > 1)
            .Select(q => q.Key)
            .ToList();

        if (duplicateParams.Count != 0)
        {
            _logger.LogWarning(
                "Duplicate query parameters detected: {Parameters}",
                string.Join(", ", duplicateParams));

            var result = Result.Failure(
                new Error(
                    "Duplicate Query Parameters",
                    ErrorType.Validation,
                    $"Duplicate query parameters are not allowed: {string.Join(", ", duplicateParams)}"
                ),
                HttpStatusCode.BadRequest);

            var actionResult = result.ToActionResult();
            await actionResult.ExecuteResultAsync(new ActionContext
            {
                HttpContext = context,
                RouteData = context.GetRouteData(),
                ActionDescriptor = new ActionDescriptor()
            });

            return;
        }

        await _next(context);
    }
}
