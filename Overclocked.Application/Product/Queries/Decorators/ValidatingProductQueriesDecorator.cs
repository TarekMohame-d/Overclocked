using FluentValidation;
using FluentValidation.Results;
using Overclocked.Application.Product.Queries.GetPagedProducts;
using Overclocked.Application.Product.Queries.GetProduct;
using Overclocked.Contracts.Product;
using Overclocked.Domain.Common.Results;

namespace Overclocked.Application.Product.Queries.Decorators;

public class ValidatingProductQueriesDecorator(IProductQueries inner,
        IValidator<GetPagedProductsQuery> getPagedValidator) : IProductQueries
{
    public async Task<Result<PagedResult<ProductPagedResponse>>> GetPagedProductsQueryHandler(
        GetPagedProductsQuery query,
        CancellationToken cancellationToken)
    {
        ValidationResult validationResult = await getPagedValidator.ValidateAsync(query, cancellationToken);

        if(!validationResult.IsValid)
        {
            var errorDictionary = validationResult
                .Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

            return Result<PagedResult<ProductPagedResponse>>.ValidationError<GetPagedProductsQuery>(errorDictionary);
        }

        Result<PagedResult<ProductPagedResponse>> result = await inner.GetPagedProductsQueryHandler(query, cancellationToken);

        return result;
    }

    public Task<Result<ProductResponse>> GetProductQueryHandler(
        GetProductQuery query,
        CancellationToken cancellationToken)
    {
        return inner.GetProductQueryHandler(query, cancellationToken);
    }
}
