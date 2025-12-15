namespace Overclocked.Application.Common.Constants;

public static class CacheKeys
{
    public static string Brand(string id) => $"brand-{id}";
    public static string AllBrands => "brands:all";

    public static string Category(string id) => $"category-{id}";
    public static string AllCategories => "categories:all";

    public static string TagPaged(int page, int pageSize, string searchTerm, string sortBy, string direction) =>
        $"tags:page={page}:size={pageSize}:searchTerm={searchTerm}:sortBy={sortBy}:direction={direction}";
    public static string TagSet => "tags:pages";

    public static string Product(string id) => $"product-{id}";
    public static string ProductPaged(
        int page,
        int pageSize,
        string sortBy,
        string direction,
        string categoryId,
        string brandId,
        string tagId,
        string searchTerm
    ) =>
        $"products:page={page}:size={pageSize}:sortBy={sortBy}:direction={direction}:categoryId={categoryId}:brandId={brandId}:searchTerm={searchTerm}:tagId={tagId}";
    public static string ProductSet => "products:pages";

    public static string Cart(string id) => $"cart-UserId={id}";
    public static string Wishlist(string id) => $"wishlist-UserId={id}";

    public static string ReviewPaged(int page, int pageSize, string sortBy, string direction) =>
        $"reviews:page={page}:size={pageSize}:sortBy={sortBy}:direction={direction}";
    public static string ReviewSet => "reviews:pages";
}
