namespace Overclocked.Api.Routing;

public abstract class TagRoutes : BaseRoute
{
    private const string Prefix = $"{Base}/tags";

    public const string GetPaged = Prefix;
    public const string Create = Prefix;
    public const string Update = $"{Prefix}/{{id:guid}}";
    public const string Delete = $"{Prefix}/{{id:guid}}";
}
