using Microsoft.Extensions.Logging;
using Overclocked.Application.Brand.Commands.Decorators;
using Overclocked.Application.Brand.Queries.GetAllBrands;
using Overclocked.Application.Brand.Queries.GetBrand;
using Overclocked.Contracts.Brand;
using Overclocked.Domain.Common.Results;
using Serilog.Context;

namespace Overclocked.Application.Brand.Queries.Decorators;

public class LoggingBrandQueriesDecorator(
    IBrandQueries inner,
    ILogger<LoggingBrandCommandsDecorator> logger) : IBrandQueries
{
    public Task<Result<BrandResponse>> GetBrandQueryHandler(
        GetBrandQuery query,
        CancellationToken cancellationToken) =>
            ExecuteWithLoggingAsync(query, () => inner.GetBrandQueryHandler(query, cancellationToken));

    public Task<Result<IEnumerable<BrandListResponse>>> GetBrandListQueryHandler(
        GetBrandListQuery query,
        CancellationToken cancellationToken) =>
            ExecuteWithLoggingAsync(query, () => inner.GetBrandListQueryHandler(query, cancellationToken));

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
