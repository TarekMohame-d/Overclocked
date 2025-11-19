using Application.Common.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog.Context;

namespace Api.ActionFilters;

public class LoggerActionFilter(ILogger<LoggerActionFilter> logger) : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        var actionName = context.ActionDescriptor.DisplayName ?? "UnknownAction";
        logger.LogInformation("Processing request {ActionName}", actionName);
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        var actionName = context.ActionDescriptor.DisplayName ?? "UnknownAction";

        if(context.Result is ObjectResult { Value: Result result })
        {
            if(result.IsSuccess)
            {
                logger.LogInformation("Completed request {ActionName}", actionName);
            }
            else
            {
                using(LogContext.PushProperty("Errors", result.Error, true))
                {
                    logger.LogError("Completed request {ActionName} with errors", actionName);
                }
            }
        }
        else // Non-Result responses
        {
            logger.LogInformation("Completed request {ActionName}", actionName);
        }
    }
}
