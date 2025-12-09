using Microsoft.Extensions.Logging;
using Overclocked.Application.Tag.Queries.GetTags;
using Overclocked.Domain.Common.Results;
using Serilog.Context;

namespace Overclocked.Application.Tag.Queries.Decorators;

public class LoggingTagQueriesDecorator(
    ITagQueries inner,
    ILogger<LoggingTagQueriesDecorator> logger) : ITagQueries
{
    public Task<Result<PagedResult<Contracts.Tag.TagListResponse>>> GetPagedTagsQueryHandler(GetPagedTagsQuery query, CancellationToken cancellationToken) =>
        ExecuteWithLoggingAsync(query, () => inner.GetPagedTagsQueryHandler(query, cancellationToken));

    private async Task<TResult> ExecuteWithLoggingAsync<TResult>(object command, Func<Task<TResult>> action)
        where TResult : Result
    {
        var commandName = command as string ?? command.GetType().Name;
        logger.LogInformation("Processing command {CommandName}", commandName);

        TResult result = await action();

        if(result.IsSuccess)
        {
            logger.LogInformation("Completed command {CommandName}", commandName);
        }
        else
        {
            using(LogContext.PushProperty("Errors", result.Error, true))
            {
                logger.LogError("Completed command {@CommandName} with errors", commandName);
            }
        }

        return result;
    }
}
