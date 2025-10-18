namespace Api.Common.Routing;

public class CategoryRoutes : BaseRoute
{
    public const string Prefix = $"{Base}/categories";

    public const string GetById = $"{Prefix}/{{id:guid}}";
    public const string GetAll = $"{Prefix}";
    public const string Create = $"{Prefix}";
    public const string Update = $"{Prefix}/{{id:guid}}";
    public const string Delete = $"{Prefix}/{{id:guid}}";
}
