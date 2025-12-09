using Microsoft.Extensions.Logging;
using Overclocked.Application.Category.Queries.GetAllCategories;
using Overclocked.Application.Category.Queries.GetCategory;
using Overclocked.Contracts.Category;
using Overclocked.Domain.Common.Results;
using Serilog.Context;

namespace Overclocked.Application.Category.Queries.Decorators;

public class LoggingCategoryQueriesDecorator(
    ICategoryQueries inner,
    ILogger<LoggingCategoryQueriesDecorator> logger) : ICategoryQueries
{
    public Task<Result<CategoryResponse>> GetCategoryQueryHandler(
        GetCategoryQuery query,
        CancellationToken cancellationToken) =>
            ExecuteWithLoggingAsync(query, () => inner.GetCategoryQueryHandler(query, cancellationToken));

    public Task<Result<IEnumerable<CategoryListResponse>>> GetCategoryListQueryHandler(
        GetCategoryListQuery query,
        CancellationToken cancellationToken) =>
            ExecuteWithLoggingAsync(query, () => inner.GetCategoryListQueryHandler(query, cancellationToken));

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
