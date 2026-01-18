using Overclocked.Domain.ProductAggregate.ValueObjects;
using Overclocked.SharedKernel.Primitives;

namespace Overclocked.Domain.WishlistAggregate.ValueObjects;

public record WishlistItem : IValueObject
{
    public ProductId ProductId { get; private set; } = null!;

    private WishlistItem() { }

    private WishlistItem(ProductId productId) => ProductId = productId;

    public static WishlistItem Create(ProductId productId) => new(productId);
}
