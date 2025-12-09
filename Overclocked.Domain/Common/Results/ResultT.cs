using System.Net;
using System.Text.Json.Serialization;
using Overclocked.Domain.Common.Enums;

namespace Overclocked.Domain.Common.Results;

public class Result<TValue> : Result
{
    public TValue? Value { get; }

    [JsonConstructor]
    private Result(bool isSuccess, Error error, HttpStatusCode statusCode, TValue? value)
        : base(isSuccess, error, statusCode)
    {
        if(isSuccess && value is null)
        {
            throw new ArgumentException("Success result must contain a non-null value", nameof(value));
        }

        Value = value;
    }

    private Result(TValue value, HttpStatusCode statusCode = HttpStatusCode.OK)
        : base(true, Error.None, statusCode)
    {
        Value = value;
    }

    private Result(Error error, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        : base(false, error, statusCode)
    {
    }

    public static Result<TValue> Success(TValue value, HttpStatusCode statusCode = HttpStatusCode.OK) =>
        new(value, statusCode);

    public static new Result<TValue> Failure(Error error, HttpStatusCode statusCode = HttpStatusCode.BadRequest) =>
        new(error, statusCode);

    public static new Result<TValue> ValidationError<T>(Dictionary<string, string[]> errors) =>
        new(
            isSuccess: false,
            error: new Error(typeof(T).Name, ErrorType.Validation, "Validation error", errors),
            statusCode: HttpStatusCode.BadRequest,
            value: default!);

    public static implicit operator Result<TValue>(TValue value) => Success(value);
    public static implicit operator Result<TValue>(Error error) => Failure(error);
}
