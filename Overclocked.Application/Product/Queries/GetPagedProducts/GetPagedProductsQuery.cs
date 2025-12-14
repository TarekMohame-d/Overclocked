using Overclocked.Application.Abstractions.Caching;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Common.Constants;
using Overclocked.Application.Common.Enums;
using Overclocked.Contracts.Product;
using Overclocked.Domain.Common.Results;

namespace Overclocked.Application.Product.Queries.GetPagedProducts;

public record GetPagedProductsQuery : IQuery<PagedResult<ProductPagedResponse>>, ICachedQuery
{
    public required int Page { get; init; } = 1;
    public required int PageSize { get; init; } = 10;
    public required string SearchTerm { get; init; } = string.Empty;
    public required string SortBy { get; init; } = string.Empty;
    public required string Direction { get; init; } = string.Empty;
    public required Guid CategoryId { get; init; } = Guid.Empty;
    public required Guid BrandId { get; init; } = Guid.Empty;
    public required Guid TagId { get; init; } = Guid.Empty;
    public ProductSortField ProductSortField => Enum.TryParse(SortBy, true, out ProductSortField parsedSortBy)
            ? parsedSortBy
            : ProductSortField.Id;
    public SortDirection SortDirection => Enum.TryParse(Direction, true, out SortDirection parsedDirection)
            ? parsedDirection
            : SortDirection.Asc;

    public string CacheKey =>
        CacheKeys.ProductPaged(
            page: Page,
            pageSize: PageSize,
            sortBy: SortBy.ToString(),
            direction: Direction.ToString(),
            categoryId: CategoryId.ToString(),
            brandId: BrandId.ToString(),
            tagId: TagId.ToString(),
            searchTerm: SearchTerm);

    public string CacheSetKey => CacheKeys.ProductSet;

    public TimeSpan Expiration => TimeSpan.FromMinutes(5);

    public static GetPagedProductsQuery ToQuery(GetPagedProductsRequest request)
    {
        return new GetPagedProductsQuery
        {
            Page = request.Page ?? 1,
            PageSize = request.PageSize ?? 10,
            SearchTerm = request.SearchTerm ?? string.Empty,
            SortBy = request.SortBy ?? string.Empty,
            Direction = request.Direction ?? string.Empty,
            CategoryId = request.CategoryId ?? Guid.Empty,
            BrandId = request.BrandId ?? Guid.Empty,
            TagId = request.TagId ?? Guid.Empty
        };
    }
}
