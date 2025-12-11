using Overclocked.Application.Product.Queries.GetPagedProducts;
using Overclocked.Application.Product.Queries.GetProduct;
using Overclocked.Contracts.Product;
using Overclocked.Domain.Common.Results;

namespace Overclocked.Application.Product.Queries;

public interface IProductQueries
{
    Task<Result<ProductResponse>> GetProductQueryHandler(GetProductQuery query, CancellationToken cancellationToken);

    Task<Result<PagedResult<ProductPagedResponse>>> GetPagedProductsQueryHandler(
        GetPagedProductsQuery query,
        CancellationToken cancellationToken);
}
