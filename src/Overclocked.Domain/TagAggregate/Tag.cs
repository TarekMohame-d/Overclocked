using Overclocked.Domain.TagAggregate.ValueObjects;
using Overclocked.SharedKernel;
using Overclocked.SharedKernel.Primitives;

namespace Overclocked.Domain.TagAggregate;

public sealed class Tag : AggregateRoot<TagId>
{
    private Tag() { }

    private Tag(TagId id, string name)
        : base(id)
    {
        Name = name;
        NormalizedName = name.ToUpperInvariant();

        DateTimeOffset now = DateTimeOffset.UtcNow;
        CreatedAt = now;
        UpdatedAt = now;
    }

    public string Name { get; private set; } = null!;
    public string NormalizedName { get; private set; } = null!;
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public static Result<Tag> Create(string name)
    {
        name = name.Trim();

        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<Tag>(TagErrors.TagNameIsRequired);

        if (name.Length > 50)
            return Result.Failure<Tag>(TagErrors.TagNameIsTooLong);

        return Result.Success(new Tag(TagId.Create(), name));
    }

    public Result Update(string name)
    {
        name = name.Trim();

        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure(TagErrors.TagNameIsRequired);

        if (name.Length > 50)
            return Result.Failure(TagErrors.TagNameIsTooLong);

        Name = name;
        NormalizedName = name.ToUpperInvariant();
        UpdatedAt = DateTimeOffset.UtcNow;

        return Result.Success();
    }
}
