using System.Net;
using Application.Common.Results;
using Microsoft.AspNetCore.Mvc;

namespace Api.Extensions;

internal static class ResultExtensions
{
    public static IActionResult ToActionResult(this Result result)
    {
        return result.StatusCode switch
        {
            HttpStatusCode.OK => new OkObjectResult(result),
            HttpStatusCode.Created => new ObjectResult(result) { StatusCode = (int)HttpStatusCode.Created },
            HttpStatusCode.BadRequest => new BadRequestObjectResult(result),
            HttpStatusCode.Unauthorized => new UnauthorizedObjectResult(result),
            HttpStatusCode.Forbidden => new ObjectResult(result) { StatusCode = (int)HttpStatusCode.Forbidden },
            HttpStatusCode.NotFound => new NotFoundObjectResult(result),
            _ => new ObjectResult(result) { StatusCode = (int)result.StatusCode }
        };
    }

    public static IActionResult ToActionResult<T>(this Result<T> result)
    {
        return result.StatusCode switch
        {
            HttpStatusCode.OK => new OkObjectResult(result),
            HttpStatusCode.Created => new ObjectResult(result) { StatusCode = (int)HttpStatusCode.Created },
            HttpStatusCode.BadRequest => new BadRequestObjectResult(result),
            HttpStatusCode.Unauthorized => new UnauthorizedObjectResult(result),
            HttpStatusCode.Forbidden => new ObjectResult(result) { StatusCode = (int)HttpStatusCode.Forbidden },
            HttpStatusCode.NotFound => new NotFoundObjectResult(result),
            _ => new ObjectResult(result) { StatusCode = (int)result.StatusCode }
        };
    }
}
