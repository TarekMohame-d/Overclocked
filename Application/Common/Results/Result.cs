using System.Net;

namespace Application.Common.Results;

public record Result
{
    // Needed for deserialization
    public Result() { }

    protected Result(bool isSuccess, Error? error, HttpStatusCode statusCode)
    {
        if((isSuccess && error is not null) || (!isSuccess && error is null))
        {
            throw new ArgumentException("Invalid error", nameof(error));
        }

        IsSuccess = isSuccess;
        Error = error;
        StatusCode = statusCode;
    }

    public bool IsSuccess { get; init; }
    public HttpStatusCode StatusCode { get; init; }
    public Error? Error { get; init; }

    public static Result Success(HttpStatusCode statusCode = HttpStatusCode.OK) => new(true, null, statusCode);

    public static Result Failure(Error error, HttpStatusCode statusCode = HttpStatusCode.BadRequest) =>
        new(false, error, statusCode);

    public static Result ValidationError<T>(Dictionary<string, string[]> errors) =>
        new(
            false,
            new Error(typeof(T).Name, ErrorType.Validation, "Validation error", errors),
            HttpStatusCode.BadRequest
        );

    public static implicit operator Result(Error error) => Failure(error);
}

public record Result<T> : Result
{
    public T? Data { get; init; }

    // Needed for deserialization
    public Result() { }

    private Result(T data, HttpStatusCode statusCode = HttpStatusCode.OK)
        : base(true, null, statusCode) => Data = data;

    private Result(Error error, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        : base(false, error, statusCode) { }

    public static Result<T> Success(T data, HttpStatusCode statusCode = HttpStatusCode.OK) => new(data, statusCode);

    public static new Result<T> Failure(Error error, HttpStatusCode statusCode = HttpStatusCode.BadRequest) =>
        new(error, statusCode);

    public static implicit operator Result<T>(T value) => new(value);

    public static implicit operator Result<T>(Error error) => new(error);
}
