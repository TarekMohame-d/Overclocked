using Overclocked.Domain.CategoryAggregate.Events;
using Overclocked.Domain.CategoryAggregate.ValueObjects;
using Overclocked.Domain.Common.Shared.ValueObjects.Image;
using Overclocked.SharedKernel;
using Overclocked.SharedKernel.Primitives;

namespace Overclocked.Domain.CategoryAggregate;

public sealed class Category : AggregateRoot<CategoryId>
{
    private Category() { }

    private Category(CategoryId id, string name, Image image)
        : base(id)
    {
        Name = name;
        NormalizedName = name.ToUpperInvariant();
        Image = image;

        DateTimeOffset now = DateTimeOffset.UtcNow;
        CreatedAt = now;
        UpdatedAt = now;
    }

    public string Name { get; private set; } = null!;
    public string NormalizedName { get; private set; } = null!;
    public Image Image { get; private set; } = null!;
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public static Result<Category> Create(string name, Image image)
    {
        name = name.Trim();

        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<Category>(CategoryErrors.CategoryNameIsRequired);

        if (name.Length > 50)
            return Result.Failure<Category>(CategoryErrors.CategoryNameIsTooLong);

        return Result.Success(new Category(CategoryId.Create(), name, image));
    }

    public Result Update(string name, Image image)
    {
        name = name.Trim();

        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure(CategoryErrors.CategoryNameIsRequired);

        if (name.Length > 50)
            return Result.Failure(CategoryErrors.CategoryNameIsTooLong);

        if (Image != image)
        {
            RaiseDomainEvent(new CategoryImageUpdatedEvent(Id.Value, Image.Value));
            Image = image;
        }

        Name = name;
        NormalizedName = name.ToUpperInvariant();
        UpdatedAt = DateTimeOffset.UtcNow;

        return Result.Success();
    }

    public void DeleteCategoryImage() => RaiseDomainEvent(new CategoryDeletedEvent(Id.Value, Image.Value));
}
