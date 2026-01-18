using Overclocked.Domain.Common.Shared.ValueObjects.Image;
using Overclocked.Domain.ProductAggregate.ValueObjects;
using Overclocked.SharedKernel.Primitives;

namespace Overclocked.Domain.ProductAggregate.Entities;

public sealed class ProductImage : Entity<ProductImageId>
{
    private ProductImage() { }

    private ProductImage(ProductImageId id, Image image)
        : base(id)
    {
        Image = image;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public Image Image { get; private set; } = null!;
    public DateTimeOffset CreatedAt { get; private set; }

    public static ProductImage Create(Image image) => new(ProductImageId.Create(), image);
}
