using Microsoft.Extensions.Logging;
using Overclocked.Application.Brand.Commands;
using Overclocked.Application.Cart.Commands.AddCartItem;
using Overclocked.Contracts.Cart;
using Overclocked.Domain.Common.Results;
using Serilog.Context;

namespace Overclocked.Application.Cart.Commands.Decorators;

public class LoggingCartCommandsDecorator(
    ICartCommands inner,
    ILogger<LoggingCartCommandsDecorator> logger) : ICartCommands
{
    public Task<Result<CartItemResponse>> AddCartItemCommandHandler(
        AddCartItemCommand command,
        CancellationToken cancellationToken) =>
        ExecuteWithLoggingAsync(command, () => inner.AddCartItemCommandHandler(command, cancellationToken));

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
