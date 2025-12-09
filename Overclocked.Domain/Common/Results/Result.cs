using System.Net;
using System.Text.Json.Serialization;
using Overclocked.Domain.Common.Enums;

namespace Overclocked.Domain.Common.Results;

public class Result
{
    [JsonConstructor]
    internal Result(bool isSuccess, Error error, HttpStatusCode statusCode)
    {
        if((isSuccess && error != Error.None) || (!isSuccess && error == Error.None))
        {
            throw new ArgumentException("Invalid error configuration", nameof(error));
        }

        IsSuccess = isSuccess;
        Error = error;
        StatusCode = statusCode;
    }

    public bool IsSuccess { get; }
    public HttpStatusCode StatusCode { get; }
    public Error Error { get; }

    public static Result Success(HttpStatusCode statusCode = HttpStatusCode.OK) =>
        new(true, Error.None, statusCode);

    public static Result Failure(Error error, HttpStatusCode statusCode = HttpStatusCode.BadRequest) =>
        new(false, error, statusCode);

    public static Result ValidationError<T>(Dictionary<string, string[]> errors) =>
        new(
            false,
            new Error(typeof(T).Name, ErrorType.Validation, "Validation error", errors),
            HttpStatusCode.BadRequest);

    public static implicit operator Result(Error error) => Failure(error);
}
