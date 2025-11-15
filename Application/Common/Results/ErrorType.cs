using System.Text.Json.Serialization;

namespace Application.Common.Results;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ErrorType
{
    NotFound,
    Validation,
    BadRequest,
    Conflict,
    Unauthorized,
    Forbidden,
    InternalServerError,
    FileStorageError
}
