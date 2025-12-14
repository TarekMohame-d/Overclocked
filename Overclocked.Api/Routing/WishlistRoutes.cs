namespace Overclocked.Api.Routing;

public abstract class WishlistRoutes : BaseRoute
{
    private const string Prefix = $"{Base}/wishlist";

    public const string GetWishlistItems = $"{Prefix}";
    public const string AddWishlistItem = $"{Prefix}";
    public const string DeleteWishlistItem = $"{Prefix}/{{id:guid}}";
    public const string ClearWishlist = $"{Prefix}/clear";
}
