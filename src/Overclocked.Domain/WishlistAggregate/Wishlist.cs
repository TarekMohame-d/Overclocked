using Overclocked.Domain.ProductAggregate.ValueObjects;
using Overclocked.Domain.UserAggregate.ValueObjects;
using Overclocked.Domain.WishlistAggregate.ValueObjects;
using Overclocked.SharedKernel.Primitives;

namespace Overclocked.Domain.WishlistAggregate;

public sealed class Wishlist : AggregateRoot<WishlistId>
{
    public UserId UserId { get; private set; } = null!;

    private readonly List<WishlistItem> _wishlistItems = [];
    public IReadOnlyCollection<WishlistItem> WishlistItems => _wishlistItems.AsReadOnly();

    private Wishlist() { }

    private Wishlist(WishlistId id, UserId userId)
        : base(id) => UserId = userId;

    public static Wishlist Create(UserId userId) => new(WishlistId.Create(), userId);

    public void AddWishlistItem(ProductId productId)
    {
        WishlistItem? existingItem = _wishlistItems.FirstOrDefault(i => i.ProductId == productId);

        if (existingItem is not null)
            return;

        var wishlistItem = WishlistItem.Create(productId);
        _wishlistItems.Add(wishlistItem);
    }

    public void RemoveWishlistItem(ProductId productId)
    {
        WishlistItem? existingItem = _wishlistItems.FirstOrDefault(i => i.ProductId == productId);
        if (existingItem is null)
            return;

        _wishlistItems.Remove(existingItem);
    }

    public void ClearWishlist() => _wishlistItems.Clear();
}
