namespace Api.Routing;

public abstract class TagRoutes : BaseRoute
{
    private const string Prefix = $"{Base}/tags";

    public const string GetById = $"{Prefix}/{{id:guid}}";
    public const string GetAll = $"{Prefix}";
    public const string Create = $"{Prefix}";
    public const string Update = $"{Prefix}/{{id:guid}}";
    public const string Delete = $"{Prefix}/{{id:guid}}";
}
