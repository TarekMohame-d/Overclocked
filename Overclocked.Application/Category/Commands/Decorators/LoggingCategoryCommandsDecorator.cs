using Microsoft.Extensions.Logging;
using Overclocked.Application.Category.Commands.CreateCategory;
using Overclocked.Application.Category.Commands.DeleteCategory;
using Overclocked.Application.Category.Commands.UpdateCategory;
using Overclocked.Domain.Common.Results;
using Serilog.Context;

namespace Overclocked.Application.Category.Commands.Decorators;

public class LoggingCategoryCommandsDecorator(
    ICategoryCommands inner,
    ILogger<LoggingCategoryCommandsDecorator> logger) : ICategoryCommands
{
    public Task<Result> CreateCategoryCommandHandler(
        CreateCategoryCommand command,
        CancellationToken cancellationToken) =>
            ExecuteWithLoggingAsync(command, () => inner.CreateCategoryCommandHandler(command, cancellationToken));

    public Task<Result> UpdateCategoryCommandHandler(
        UpdateCategoryCommand command,
        CancellationToken cancellationToken) =>
            ExecuteWithLoggingAsync(command, () => inner.UpdateCategoryCommandHandler(command, cancellationToken));

    public Task<Result> DeleteCategoryCommandHandler(
        DeleteCategoryCommand command,
        CancellationToken cancellationToken) =>
            ExecuteWithLoggingAsync(command, () => inner.DeleteCategoryCommandHandler(command, cancellationToken));

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
