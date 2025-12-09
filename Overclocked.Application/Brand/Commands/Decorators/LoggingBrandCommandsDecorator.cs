using Microsoft.Extensions.Logging;
using Overclocked.Application.Brand.Commands.CreateBrand;
using Overclocked.Application.Brand.Commands.DeleteBrand;
using Overclocked.Application.Brand.Commands.UpdateBrand;
using Overclocked.Domain.Common.Results;
using Serilog.Context;

namespace Overclocked.Application.Brand.Commands.Decorators;

public class LoggingBrandCommandsDecorator(
    IBrandCommands inner,
    ILogger<LoggingBrandCommandsDecorator> logger) : IBrandCommands
{
    public Task<Result> CreateBrandCommandHandler(CreateBrandCommand command, CancellationToken cancellationToken) =>
        ExecuteWithLoggingAsync(command, () => inner.CreateBrandCommandHandler(command, cancellationToken));

    public Task<Result> UpdateBrandCommandHandler(UpdateBrandCommand command, CancellationToken cancellationToken) =>
        ExecuteWithLoggingAsync(command, () => inner.UpdateBrandCommandHandler(command, cancellationToken));

    public Task<Result> DeleteBrandCommandHandler(DeleteBrandCommand command, CancellationToken cancellationToken) =>
        ExecuteWithLoggingAsync(command, () =>
            inner.DeleteBrandCommandHandler(command, cancellationToken));

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
