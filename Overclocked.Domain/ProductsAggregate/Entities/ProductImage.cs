using Overclocked.Domain.Common.Primitives;
using Overclocked.Domain.ProductsAggregate.ValueObjects;

namespace Overclocked.Domain.ProductsAggregate.Entities;

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

    internal static ProductImage Create(ProductImageId id, string imageUrl) =>
        new(id: id, imageUrl: imageUrl);
}
