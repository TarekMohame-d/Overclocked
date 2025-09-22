namespace Api.Common.Routing;

public class CategoryRoutes : BaseRoute
{
    public const string Prefix = $"{Base}/category";

    public const string GetById = $"{Prefix}/{{id:guid}}";
    public const string GetAll = $"{Base}/categories";
    public const string Create = $"{Prefix}";
    public const string Update = $"{Prefix}/{{id:guid}}";
    public const string Delete = $"{Prefix}/{{id:guid}}";
}
