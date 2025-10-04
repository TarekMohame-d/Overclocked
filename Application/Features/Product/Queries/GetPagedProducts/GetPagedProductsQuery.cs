using Application.Abstraction.Messaging;
using Application.Common.Constants;
using Application.Common.Results;

namespace Application.Features.Product.Queries.GetPagedProducts;

public record GetPagedProductsQuery : GetPagedProductsRequest, ICachedQuery<Result<PagedResult<ProductListDto>>>, IValidation
{
    public string CacheKey => CacheKeys.ProductPaged(Page, PageSize, SortBy);
    public string CacheSetKey => CacheKeys.ProductSet;
    public bool BypassCache => false;
}

public record GetPagedProductsRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string SortBy { get; set; } = "id_asc";
}
