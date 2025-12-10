using Microsoft.Extensions.Logging;
using Overclocked.Application.Product.Commands.CreateProduct;
using Overclocked.Application.Product.Commands.DeleteProduct;
using Overclocked.Application.Product.Commands.UpdateProduct;
using Overclocked.Domain.Common.Results;
using Serilog.Context;

namespace Overclocked.Application.Product.Commands.Decorators;

public class LoggingProductCommandsDecorator(
    IProductCommands inner,
    ILogger<LoggingProductCommandsDecorator> logger) : IProductCommands
{
    public Task<Result> CreateProductCommandHandler(
        CreateProductCommand command,
        CancellationToken cancellationToken) =>
            ExecuteWithLoggingAsync(command, () => inner.CreateProductCommandHandler(command, cancellationToken));

    public Task<Result> UpdateProductCommandHandler(
        UpdateProductCommand command,
        CancellationToken cancellationToken) =>
            ExecuteWithLoggingAsync(command, () => inner.UpdateProductCommandHandler(command, cancellationToken));

    public Task<Result> DeleteProductCommandHandler(
        DeleteProductCommand command,
        CancellationToken cancellationToken) =>
        ExecuteWithLoggingAsync(command, () => inner.DeleteProductCommandHandler(command, cancellationToken));

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
