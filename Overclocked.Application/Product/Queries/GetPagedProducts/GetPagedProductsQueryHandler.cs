using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Product.Mapping;
using Overclocked.Contracts.Product;
using Overclocked.Domain.Common.Results;
using ProductEntity = Overclocked.Domain.ProductAggregate.Product;

namespace Overclocked.Application.Product.Queries.GetPagedProducts;

public class GetPagedProductsQueryHandler(IProductRepository productRepository)
    : IQueryHandler<GetPagedProductsQuery, PagedResult<ProductPagedResponse>>
{
    public async Task<Result<PagedResult<ProductPagedResponse>>> Handle(
        GetPagedProductsQuery query,
        CancellationToken cancellationToken)
    {
        var totalCount = await productRepository.CountAsync(
            query.SearchTerm,
            query.BrandId,
            query.CategoryId,
            query.TagId,
            cancellationToken);

        if(totalCount == 0)
        {
            return Result.Success(PagedResult<ProductPagedResponse>.Empty(query.Page, query.PageSize));
        }

        List<ProductEntity> products = await productRepository.GetPagedAsync(
            pageNumber: query.Page,
            pageSize: query.PageSize,
            searchTerm: query.SearchTerm,
            brandId: query.BrandId,
            categoryId: query.CategoryId,
            tagId: query.TagId,
            sortBy: query.ProductSortField,
            direction: query.SortDirection,
            cancellationToken: cancellationToken);

        return Result.Success(PagedResult<ProductPagedResponse>.Create(
                products.ToDto(),
                query.Page,
                query.PageSize,
                totalCount));
    }
}
