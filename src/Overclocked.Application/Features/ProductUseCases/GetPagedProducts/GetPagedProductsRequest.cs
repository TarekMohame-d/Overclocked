using Overclocked.Application.Abstractions.Caching;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Common.Constants;
using Overclocked.Application.Common.Enums;
using Overclocked.Application.Features.ProductUseCases.DTOs.Requests;
using Overclocked.Application.Features.ProductUseCases.DTOs.Responses;
using Overclocked.SharedKernel;

namespace Overclocked.Application.Features.ProductUseCases.GetPagedProducts;

public record GetPagedProductsRequest : IRequest<PagedResult<ProductPagedResponse>>, ICachedRequest
{
    public required int Page { get; init; }
    public required int PageSize { get; init; }
    public required string SearchTerm { get; init; }
    public required string SortBy { get; init; }
    public required string Direction { get; init; }
    public required Guid CategoryId { get; init; }
    public required Guid BrandId { get; init; }
    public required Guid TagId { get; init; }
    public bool HasDiscount { get; init; }

    public ProductSortField ProductSortField =>
        Enum.TryParse(SortBy, true, out ProductSortField parsedSortBy) ? parsedSortBy : ProductSortField.Id;
    public SortDirection SortDirection =>
        Enum.TryParse(Direction, true, out SortDirection parsedDirection) ? parsedDirection : SortDirection.Asc;

    public string CacheKey =>
        CacheKeys.ProductPaged(
            page: Page,
            pageSize: PageSize,
            sortBy: ProductSortField.ToString().ToLower(),
            direction: SortDirection.ToString().ToLower(),
            categoryId: CategoryId.ToString(),
            brandId: BrandId.ToString(),
            tagId: TagId.ToString(),
            searchTerm: SearchTerm
        );
    public string CacheSetKey => CacheKeys.ProductSet;
    public TimeSpan Expiration => TimeSpan.FromMinutes(5);

    public static GetPagedProductsRequest FromRequest(GetPagedProductsQuery query) =>
        new()
        {
            Page = query.Page ?? 1,
            PageSize = query.PageSize ?? 10,
            SearchTerm = query.SearchTerm ?? string.Empty,
            SortBy = query.SortBy ?? string.Empty,
            Direction = query.Direction ?? string.Empty,
            HasDiscount = query.HasDiscount ?? false,
            CategoryId = query.CategoryId ?? Guid.Empty,
            BrandId = query.BrandId ?? Guid.Empty,
            TagId = query.TagId ?? Guid.Empty,
        };
}
