using Overclocked.Domain.Common.Primitives;
using Overclocked.Domain.ProductAggregate.ValueObjects;
using Overclocked.Domain.UserAggregate.ValueObjects;
using Overclocked.Domain.WishlistAggregate.ValueObjects;

namespace Overclocked.Domain.WishlistAggregate;

public class Wishlist : AggregateRoot<WishlistId>
{
    public UserId UserId { get; private set; }

    private readonly List<WishlistItem> _wishlistItems = [];
    public IReadOnlyCollection<WishlistItem> WishlistItems => _wishlistItems.AsReadOnly();

    private Wishlist()
    {
    }
    private Wishlist(WishlistId id, UserId userId) : base(id)
    {
        UserId = userId;
    }

    public static Wishlist Create(WishlistId id, UserId userId)
    {
        return new(id, userId);
    }

    public void AddWishlistItem(ProductId productId)
    {
        WishlistItem? existingItem = _wishlistItems.FirstOrDefault(i => i.ProductId == productId);

        if(existingItem is null)
        {
            var wishlistItem = WishlistItem.Create(productId);
            _wishlistItems.Add(wishlistItem);
        }
    }

    public void RemoveWishlistItem(ProductId productId)
    {
        WishlistItem? existingItem = _wishlistItems.FirstOrDefault(i => i.ProductId == productId);
        if(existingItem is not null)
        {
            _wishlistItems.Remove(existingItem);
        }
    }

    public void ClearWishlist()
    {
        _wishlistItems.Clear();
    }
}
