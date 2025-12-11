using Microsoft.Extensions.Logging;
using Overclocked.Application.Brand.Commands.Decorators;
using Overclocked.Application.Product.Queries.GetPagedProducts;
using Overclocked.Application.Product.Queries.GetProduct;
using Overclocked.Contracts.Product;
using Overclocked.Domain.Common.Results;
using Serilog.Context;

namespace Overclocked.Application.Product.Queries.Decorators;

public class LoggingProductQueriesDecorator(
    IProductQueries inner,
    ILogger<LoggingBrandCommandsDecorator> logger) : IProductQueries
{
    public Task<Result<ProductResponse>> GetProductQueryHandler(
        GetProductQuery query,
        CancellationToken cancellationToken) =>
            ExecuteWithLoggingAsync(query, () => inner.GetProductQueryHandler(query, cancellationToken));

    public Task<Result<PagedResult<ProductPagedResponse>>> GetPagedProductsQueryHandler(
        GetPagedProductsQuery query,
        CancellationToken cancellationToken) =>
            ExecuteWithLoggingAsync(query, () => inner.GetPagedProductsQueryHandler(query, cancellationToken));

    private async Task<TResult> ExecuteWithLoggingAsync<TResult>(object query, Func<Task<TResult>> action)
        where TResult : Result
    {
        var queryName = query as string ?? query.GetType().Name;
        logger.LogInformation("Processing query {QueryName}", queryName);

        TResult result = await action();

        if(result.IsSuccess)
        {
            logger.LogInformation("Completed query {QueryName}", queryName);
        }
        else
        {
            using(LogContext.PushProperty("Errors", result.Error, true))
            {
                logger.LogError("Completed query {@QueryName} with errors", queryName);
            }
        }

        return result;
    }
}
