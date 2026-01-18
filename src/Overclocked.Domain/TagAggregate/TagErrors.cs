using Overclocked.SharedKernel;

namespace Overclocked.Domain.TagAggregate;

public static class TagErrors
{
    public static Error TagNotFound(Guid id) => Error.NotFound("Tag.NotFound", $"The tag with ID '{id}' was not found.");

    public static Error TagsNotFound(List<Guid> ids)
    {
        var formatted = string.Join(", ", ids);
        return Error.NotFound("Tags.NotFound", $"The tags with IDs '{formatted}' were not found.");
    }

    public static Error TagNameAlreadyExists => Error.Conflict("Tag.NameAlreadyExists", "Name already exists.");

    public static Error TagNameIsRequired => Error.Validation("Tag.NameIsRequired", "Name is required.");

    public static Error TagNameIsTooLong => Error.Validation("Tag.NameIsTooLong", "Name must be less than 50 characters.");
}
