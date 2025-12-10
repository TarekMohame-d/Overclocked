using Overclocked.Domain.Common.Primitives;
using Overclocked.Domain.ProductAggregate.ValueObjects;

namespace Overclocked.Domain.ProductAggregate.Entities;

public sealed class ProductImage : Entity<ProductImageId>
{
    private ProductImage()
    {
    }

    private ProductImage(ProductImageId id, string imageUrl) : base(id)
    {
        ImageUrl = imageUrl;
        CreatedAt = DateTime.UtcNow;
    }

    public string ImageUrl { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public static ProductImage Create(ProductImageId id, string imageUrl) =>
        new(id: id, imageUrl: imageUrl);
}
