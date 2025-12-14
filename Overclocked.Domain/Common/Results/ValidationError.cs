using Overclocked.Domain.Common.Enums;

namespace Overclocked.Domain.Common.Results;

public sealed record ValidationError : Error
{
    public ValidationError(Dictionary<string, string[]> errors)
        : base(
            "Validation.General",
            "One or more validation errors occurred",
            ErrorType.Validation)
    {
        Errors = errors;
    }

    public Dictionary<string, string[]> Errors { get; }
}
