using Application.Common.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog.Context;

namespace Api.ActionFilters;

public class LoggerActionFilter : IActionFilter
{
    private readonly ILogger<LoggerActionFilter> _logger;

    public LoggerActionFilter(ILogger<LoggerActionFilter> logger)
    {
        _logger = logger;
    }
    public void OnActionExecuting(ActionExecutingContext context)
    {
        string actionName = context.ActionDescriptor.DisplayName ?? "UnknownAction";
        _logger.LogInformation("Processing request {ActionName}", actionName);
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        string actionName = context.ActionDescriptor.DisplayName ?? "UnknownAction";

        if (context.Result is ObjectResult objectResult &&
            objectResult.Value is Result result)
        {
            if (result.IsSuccess)
            {
                _logger.LogInformation("Completed request {ActionName}", actionName);
            }
            else
            {
                using (LogContext.PushProperty("Errors", result.Error, true))
                {
                    _logger.LogError("Completed request {ActionName} with errors", actionName);
                }
            }
        }
        else
        {
            // Non-Result responses
            _logger.LogInformation("Completed request {ActionName}", actionName);
        }
    }
}
