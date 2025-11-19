using Application.Common.Results;
using Application.Services.Product.DTOs.Request;
using Application.Services.Product.DTOs.Response;
using Application.Services.Product.Mapping;

namespace Application.Services.Product;

public sealed partial class ProductService
{
    public async Task<Result<PagedResult<ProductListResponse>>> GetPagedProductsAsync(
        GetPagedProductsRequest request,
        CancellationToken cancellationToken
    )
    {
        IQueryable<Domain.Entities.Product> productsQuery = productRepository.GetProductsQuery(
            request.SortBy,
            request.Direction,
            request.Search,
            request.Category,
            request.Brand,
            request.TagId
        );

        IQueryable<ProductListResponse> productsDtoQuery = productsQuery.ToDto();

        PagedResult<ProductListResponse> pagedResult = await PagedResult<ProductListResponse>.CreateAsync(
            productsDtoQuery,
            request.Page,
            request.PageSize
        );

        return Result<PagedResult<ProductListResponse>>.Success(pagedResult);
    }
}
