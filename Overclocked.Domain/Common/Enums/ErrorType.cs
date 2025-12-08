using System.Text.Json.Serialization;

namespace Overclocked.Domain.Common.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ErrorType
{
    None,
    Failure,
    NotFound,
    Validation,
    BadRequest,
    Conflict,
    Unauthorized,
    Forbidden,
    InternalServerError,
    FileStorageError
}
