namespace Overclocked.Api.Routing;

public abstract class PaymentRoutes : BaseRoute
{
    private const string Prefix = $"{Base}/payments";

    public const string GetPayments = Prefix;
    public const string PaymobCallback = $"{Prefix}/paymob-callback";
}
