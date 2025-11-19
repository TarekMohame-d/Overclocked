namespace Application.Common.Results;

public sealed record Error(
    string Code,
    ErrorType Type,
    string Description,
    Dictionary<string, string[]>? ValidationErrors = null
);
