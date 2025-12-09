using Overclocked.Domain.Common.Enums;

namespace Overclocked.Domain.Common.Results;

public sealed record Error(
    string Code,
    ErrorType Type,
    string Description,
    Dictionary<string, string[]>? ValidationErrors = null)
{
    public static readonly Error None = new(string.Empty, ErrorType.None, string.Empty);
}
