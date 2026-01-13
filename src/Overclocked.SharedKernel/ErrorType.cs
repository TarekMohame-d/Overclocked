using System.Text.Json.Serialization;

namespace Overclocked.SharedKernel;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ErrorType
{
    None = 0,
    Failure,
    Problem,
    NotFound,
    Validation,
    BadRequest,
    Conflict,
    Unauthorized,
    Forbidden,
    InternalServerError,
    FileStorageError,
}
