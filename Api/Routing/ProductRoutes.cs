using Api.Common.Routing;

namespace Api.Routing;

public class ProductRoutes : BaseRoute
{
    public const string Prefix = $"{Base}/products";

    public const string GetById = $"{Prefix}/{{id:guid}}";
    public const string GetAll = $"{Prefix}";
    public const string Create = $"{Prefix}";
    public const string Update = $"{Prefix}/{{id:guid}}";
    public const string Delete = $"{Prefix}/{{id:guid}}";
}
