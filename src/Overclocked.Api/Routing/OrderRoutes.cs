namespace Overclocked.Api.Routing;

public abstract class OrderRoutes : BaseRoute
{
    private const string Prefix = $"{Base}/orders";

    public const string CreateOrder = Prefix;
    public const string RetryOrder = $"{Prefix}/{{id:Guid}}";
    public const string CancelOrder = $"{Prefix}/{{id:Guid}}/cancel";
    public const string GetPagedOrders = $"{Prefix}/{{year:int}}";
}
