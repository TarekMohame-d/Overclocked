namespace Application.Common.Results.PredefinedErrors;

public static partial class Errors
{
    public static readonly Error TagNotFound = new(nameof(TagNotFound), ErrorType.NotFound, "Tag not found.");

    public static readonly Error TagNameAlreadyExists = new(
        nameof(TagNameAlreadyExists),
        ErrorType.Conflict,
        "Tag name already exists."
    );
}
