using Overclocked.Domain.Common.Enums;
using Overclocked.Domain.Common.Results;

namespace Overclocked.Domain.Common.Errors;

public static class TagErrors
{
    public static Error TagNotFound(Guid id)
    {
        return new(nameof(TagNotFound), ErrorType.NotFound, $"The tag with ID '{id}' was not found.");
    }

    public static readonly Error TagNameAlreadyExists = new(
        nameof(TagNameAlreadyExists),
        ErrorType.Conflict,
        "Tag name already exists.");
}
