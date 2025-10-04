using Application.Abstraction.Messaging;
using Application.Abstraction.Repositories;
using Application.Common.Results;
using Application.Features.Product.Mapping;

namespace Application.Features.Product.Queries.GetPagedProducts;

public class GetPagedProductsQueryHandler : IQueryHandler<GetPagedProductsQuery, Result<PagedResult<ProductListDto>>>
{
    private readonly IProductRepository _productRepository;

    public GetPagedProductsQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<Result<PagedResult<ProductListDto>>> Handle(GetPagedProductsQuery query, CancellationToken cancellationToken)
    {
        var productsQuery = _productRepository.GetProductsQuery(query.SortBy);
        var productsDtoQuery = productsQuery.ToDto();
        var pagedResult = await PagedResult<ProductListDto>.CreateAsync(productsDtoQuery, query.Page, query.PageSize);

        return Result<PagedResult<ProductListDto>>.Success(pagedResult);
    }
}
