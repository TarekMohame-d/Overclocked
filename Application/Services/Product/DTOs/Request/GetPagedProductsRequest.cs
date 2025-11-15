using Application.Abstraction.Messaging;
using Application.Common.Constants;
using Application.Common.Enums;

namespace Application.Services.Product.DTOs.Request;

public record GetPagedProductsQuery
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string SortBy { get; init; } = "id";
    public string Direction { get; init; } = "asc";
    public string? Search { get; init; }
    public string? Category { get; init; }
    public string? Brand { get; init; }
    public Guid? TagId { get; init; }
}

public record GetPagedProductsRequest : GetPagedProductsQuery, ICachedRequest
{
    public new ProductSortField SortBy { get; private init; }
    public new SortDirection Direction { get; private init; }

    public string CacheKey => CacheKeys.ProductPaged(
        Page, PageSize, SortBy.ToString(), Direction.ToString(),
        Category ?? "all", Brand ?? "all", TagId?.ToString() ?? "all", Search ?? "all"
    );

    public string CacheSetKey => CacheKeys.ProductSet;
    public TimeSpan SlidingExpiration => TimeSpan.FromMinutes(5);

    public static GetPagedProductsRequest FromQuery(GetPagedProductsQuery query)
    {
        ProductSortField sortBy = Enum.TryParse(query.SortBy, true, out ProductSortField parsedSortBy)
            ? parsedSortBy
            : ProductSortField.Id;

        SortDirection direction = Enum.TryParse(query.Direction, true, out SortDirection parsedDirection)
            ? parsedDirection
            : SortDirection.Asc;

        return new GetPagedProductsRequest
        {
            Page = query.Page,
            PageSize = query.PageSize,
            SortBy = sortBy,
            Direction = direction,
            Search = query.Search,
            Category = query.Category,
            Brand = query.Brand,
            TagId = query.TagId
        };
    }
}
