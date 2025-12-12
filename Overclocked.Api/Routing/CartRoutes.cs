namespace Overclocked.Api.Routing;

public abstract class CartRoutes : BaseRoute
{
    private const string Prefix = $"{Base}/cart";

    public const string GetCartItems = $"{Prefix}";
    public const string AddCartItem = $"{Prefix}";
    public const string UpdateCartItem = $"{Prefix}/{{id:guid}}";
    public const string DeleteCartItem = $"{Prefix}/{{id:guid}}";
    public const string ClearCart = $"{Prefix}/clear";
}
