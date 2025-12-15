namespace Overclocked.Api.Routing;

public abstract class ReviewRoutes : BaseRoute
{
    private const string Prefix = $"{Base}/products/{{productId:guid}}/reviews";

    public const string GetReviewsRatingBreakdown = $"{Prefix}/rating-breakdown";
    public const string GetPaged = $"{Prefix}";
    public const string Create = $"{Prefix}";
    public const string Update = $"{Prefix}/{{id:guid}}";
    public const string Delete = $"{Prefix}/{{id:guid}}";
}
