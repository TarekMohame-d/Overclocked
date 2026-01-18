namespace Overclocked.Api.Routing;

public class UserRoutes : BaseRoute
{
    private const string Prefix = $"{Base}/users";

    public const string AddAddress = $"{Prefix}/address";
    public const string DeleteAddress = $"{Prefix}/address";
    public const string GetAllAddresses = $"{Prefix}/address";
}
