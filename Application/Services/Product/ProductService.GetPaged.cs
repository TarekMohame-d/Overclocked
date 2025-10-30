using Application.Common.Results;
using Application.Features.Brand.Mapping;
using Application.Features.Product.Mapping;
using Application.Features.Tag.Mapping;
using Application.Services.Product.DTOs.Request;
using Application.Services.Product.DTOs.Response;

namespace Application.Services.Product;

public sealed partial class ProductService
{
    public async Task<Result<PagedResult<ProductListResponse>>> GetPagedProductsAsync(GetPagedProductsQuery query, CancellationToken cancellationToken)
    {
        var productsQuery = _productRepository.GetProductsQuery(
            query.SortBy,
            query.Direction,
            query.Search,
            query.Category,
            query.Brand,
            query.TagId
        );

        var productsDtoQuery = productsQuery.ToDto();

        var pagedResult = await PagedResult<ProductListResponse>.CreateAsync(
            productsDtoQuery,
            query.Page,
            query.PageSize
        );

        return Result<PagedResult<ProductListResponse>>.Success(pagedResult);
    }
}
