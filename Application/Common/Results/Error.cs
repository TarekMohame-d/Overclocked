using System.Text.Json.Serialization;

namespace Application.Common.Results;

public sealed record Error
{
    public string Id { get; init; }
    public ErrorType Type { get; init; }
    public string Description { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string, string[]>? ValidationErrors { get; init; }

    public Error(string id, ErrorType type, string description, Dictionary<string, string[]>? validationErrors = default)
    {
        Id = id;
        Type = type;
        Description = description;
        ValidationErrors = validationErrors;
    }
}

// Predefined errors (avoids magic strings)
public static class Errors
{
    public static readonly Error EmailAlreadyExists = new(nameof(EmailAlreadyExists), ErrorType.Conflict, "Email already exists, please login.");
    public static readonly Error PhoneNumberAlreadyExists = new(nameof(PhoneNumberAlreadyExists), ErrorType.Conflict, "Phone number already exists.");


    public static readonly Error FileStorageError = new(nameof(FileStorageError), ErrorType.FileStorageError, "Upload file failed.");
    public static readonly Error InternalServerError = new(nameof(InternalServerError), ErrorType.InternalServerError, "An error occurred.");

    // Brand errors
    public static readonly Error BrandNotFound = new(nameof(BrandNotFound), ErrorType.NotFound, "Brand not found.");
    public static readonly Error BrandNameAlreadyExists = new(nameof(BrandNameAlreadyExists), ErrorType.Conflict, "Brand name already exists.");

    // Category errors
    public static readonly Error CategoryNotFound = new(nameof(CategoryNotFound), ErrorType.NotFound, "Category not found.");
    public static readonly Error CategoryNameAlreadyExists = new(nameof(CategoryNameAlreadyExists), ErrorType.Conflict, "Category name already exists.");

    // Tag errors
    public static readonly Error TagNotFound = new(nameof(TagNotFound), ErrorType.NotFound, "Tag not found.");
    public static readonly Error TagNameAlreadyExists = new(nameof(TagNameAlreadyExists), ErrorType.Conflict, "Tag name already exists.");
}
