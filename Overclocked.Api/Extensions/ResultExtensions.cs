using Microsoft.AspNetCore.Mvc;
using Overclocked.Domain.Common.Enums;
using Overclocked.Domain.Common.Results;

namespace Overclocked.Api.Extensions;

internal static class ResultExtensions
{
    public static IActionResult ToActionResult(this Result result, ControllerBase controller)
    {
        return result.IsSuccess
            ? controller.StatusCode((int)result.StatusCode)
            : ToProblemDetails(result, controller);
    }

    public static IActionResult ToActionResult<T>(this Result<T> result, ControllerBase controller)
    {
        return result.IsSuccess
            ? controller.StatusCode((int)result.StatusCode, result.Value)
            : ToProblemDetails(result, controller);
    }

    private static IActionResult ToProblemDetails(this Result result, ControllerBase controller)
    {
        var extensions = new Dictionary<string, object?>
            {
                { "errorCode", result.Error!.Code }
            };

        if(result.Error.Type == ErrorType.Validation)
        {
            extensions.Add("errors", result.Error.ValidationErrors);
        }

        return controller.Problem(
            statusCode: GetStatusCode(result.Error!.Type),
            detail: result.Error.Description,
            extensions: extensions);

        static int GetStatusCode(ErrorType errorType) =>
            errorType switch
            {
                ErrorType.Validation or ErrorType.BadRequest => StatusCodes.Status400BadRequest,
                ErrorType.NotFound => StatusCodes.Status404NotFound,
                ErrorType.Conflict => StatusCodes.Status409Conflict,
                _ => StatusCodes.Status500InternalServerError
            };
    }
}
