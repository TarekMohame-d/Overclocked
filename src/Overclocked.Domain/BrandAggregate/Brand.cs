using Overclocked.Domain.BrandAggregate.Events;
using Overclocked.Domain.BrandAggregate.ValueObjects;
using Overclocked.Domain.Common.Shared.ValueObjects.Image;
using Overclocked.SharedKernel;
using Overclocked.SharedKernel.Primitives;

namespace Overclocked.Domain.BrandAggregate;

public sealed class Brand : AggregateRoot<BrandId>
{
    private Brand() { }

    private Brand(BrandId id, string name, Image image)
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

    public static Result<Brand> Create(string name, Image image)
    {
        name = name.Trim();

        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<Brand>(BrandErrors.BrandNameIsRequired);

        if (name.Length > 50)
            return Result.Failure<Brand>(BrandErrors.BrandNameIsTooLong);

        return Result.Success(new Brand(BrandId.Create(), name, image));
    }

    public Result Update(string name, Image image)
    {
        name = name.Trim();

        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure(BrandErrors.BrandNameIsRequired);

        if (name.Length > 50)
            return Result.Failure(BrandErrors.BrandNameIsTooLong);

        if (Image != image)
        {
            RaiseDomainEvent(new BrandImageUpdatedEvent(Id.Value, Image.Value));
            Image = image;
        }

        Name = name;
        NormalizedName = name.ToUpperInvariant();
        UpdatedAt = DateTimeOffset.UtcNow;

        return Result.Success();
    }

    public void DeleteBrandImage() => RaiseDomainEvent(new BrandDeletedEvent(Id.Value, Image.Value));
}
