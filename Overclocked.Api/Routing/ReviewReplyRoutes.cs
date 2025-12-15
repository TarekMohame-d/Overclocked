namespace Overclocked.Api.Routing;

public class ReviewReplyRoutes : BaseRoute
{
    private const string Prefix = $"{Base}/products/{{productId:guid}}/reviews/{{reviewId:guid}}/replies";

    public const string Create = $"{Prefix}";
    public const string Update = $"{Prefix}/{{replyId:guid}}";
    public const string Delete = $"{Prefix}/{{replyId:guid}}";
}
