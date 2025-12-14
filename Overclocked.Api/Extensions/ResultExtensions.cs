using Microsoft.AspNetCore.Mvc;
using Overclocked.Domain.Common.Enums;
using Overclocked.Domain.Common.Results;

namespace Overclocked.Api.Extensions;

internal static class ResultExtensions
{
    public static IActionResult Match<T>(
        this Result<T> result,
        Func<T, IActionResult> onSuccess,
        Func<Error, IActionResult> onFailure)
    {
        return result.IsSuccess ? onSuccess(result.Value) : onFailure(result.Error);
    }

    public static IActionResult Match(
        this Result result,
        Func<IActionResult> onSuccess,
        Func<Error, IActionResult> onFailure)
    {
        return result.IsSuccess ? onSuccess() : onFailure(result.Error);
    }

    internal static IActionResult ToProblemDetails(this Error error, ControllerBase controller)
    {
        var statusCode = error.Type switch
        {
            ErrorType.Validation or ErrorType.BadRequest => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError
        };

        var extensions = new Dictionary<string, object?>
            {
                { "errorCode", error!.Code }
            };

        if(error is ValidationError validationError)
        {
            extensions.Add("errors", validationError.Errors);
        }

        return controller.Problem(
            statusCode: statusCode,
            detail: error.Description,
            extensions: extensions);
    }
}
