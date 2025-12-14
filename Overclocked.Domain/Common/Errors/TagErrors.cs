using Overclocked.Domain.Common.Enums;
using Overclocked.Domain.Common.Results;

namespace Overclocked.Domain.Common.Errors;

public static class TagErrors
{
    public static Error TagNotFound(Guid id)
    {
        return new("Tag.NotFound", $"The tag with ID '{id}' was not found.", ErrorType.NotFound);
    }

    public static readonly Error TagNameAlreadyExists = new(
        "Tag.NameAlreadyExists",
        "Tag name already exists.",
        ErrorType.Conflict);
}
