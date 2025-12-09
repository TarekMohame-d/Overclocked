using Microsoft.Extensions.Logging;
using Overclocked.Application.Tag.Commands.CreateTag;
using Overclocked.Application.Tag.Commands.DeleteTag;
using Overclocked.Application.Tag.Commands.UpdateTag;
using Overclocked.Domain.Common.Results;
using Serilog.Context;

namespace Overclocked.Application.Tag.Commands.Decorators;

public class LoggingTagCommandsDecorator(
    ITagCommands inner,
    ILogger<LoggingTagCommandsDecorator> logger) : ITagCommands
{
    public Task<Result> CreateTagCommandHandler(CreateTagCommand command, CancellationToken cancellationToken) =>
        ExecuteWithLoggingAsync(command, () => inner.CreateTagCommandHandler(command, cancellationToken));

    public Task<Result> UpdateTagCommandHandler(UpdateTagCommand command, CancellationToken cancellationToken) =>
        ExecuteWithLoggingAsync(command, () => inner.UpdateTagCommandHandler(command, cancellationToken));

    public Task<Result> DeleteTagCommandHandler(DeleteTagCommand command, CancellationToken cancellationToken) =>
        ExecuteWithLoggingAsync(command, () =>
            inner.DeleteTagCommandHandler(command, cancellationToken));

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
