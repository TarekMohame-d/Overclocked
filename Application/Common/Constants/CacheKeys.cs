namespace Application.Common.Constants;

public static class CacheKeys
{
    public static string Brand(string id) => $"brand-{id}";

    public static string AllBrands => "brands:all";

    public static string Category(string id) => $"category-{id}";

    public static string AllCategories => "categories:all";

    public static string Tag(string id) => $"tag-{id}";

    public static string TagPaged(int page, int pageSize, string sortBy, string direction) =>
        $"tags:page={page}:size={pageSize}:sortBy={sortBy}:direction={direction}";

    public static string TagSet => "tags:pages";

    public static string Product(string id) => $"product-{id}";

    public static string ProductPaged(
        int page,
        int pageSize,
        string sortBy,
        string direction,
        string category,
        string brand,
        string tagId,
        string searchTerm
    ) =>
        $"products:page={page}:size={pageSize}:sortBy={sortBy}:direction={direction}:category={category}:brand={brand}:searchTerm={searchTerm}:tagId={tagId}";

    public static string ProductSet => "products:pages";
}
