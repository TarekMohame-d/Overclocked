using Microsoft.Extensions.Logging;
using Overclocked.Application.Abstraction.Services;
using Overclocked.Application.Common.Constants;
using Overclocked.Application.Product.Commands.CreateProduct;
using Overclocked.Application.Product.Commands.DeleteProduct;
using Overclocked.Application.Product.Commands.UpdateProduct;
using Overclocked.Domain.Common.Results;

namespace Overclocked.Application.Product.Commands.Decorators;

public class CachingProductCommandsDecorator(
    IProductCommands inner,
    ICacheService cacheService,
    ILogger<CachingProductCommandsDecorator> logger) : IProductCommands
{
    public async Task<Result> CreateProductCommandHandler(
        CreateProductCommand command,
        CancellationToken cancellationToken)
    {
        return await RemoveCache(() => inner.CreateProductCommandHandler(command, cancellationToken));
    }

    public async Task<Result> UpdateProductCommandHandler(
        UpdateProductCommand command,
        CancellationToken cancellationToken)
    {
        return await RemoveCache(() => inner.UpdateProductCommandHandler(command, cancellationToken));
    }

    public async Task<Result> DeleteProductCommandHandler(
        DeleteProductCommand command,
        CancellationToken cancellationToken)
    {
        return await RemoveCache(() => inner.DeleteProductCommandHandler(command, cancellationToken));
    }

    private async Task<TResult> RemoveCache<TResult>(Func<Task<TResult>> action)
        where TResult : Result
    {
        TResult result = await action();

        if(result.IsSuccess)
        {
            logger.LogInformation("Removing cache for Product set: {SetKey}", CacheKeys.ProductSet);
            await cacheService.RemoveKeysInSetAsync(CacheKeys.ProductSet);
            logger.LogInformation("Removed cache for Product set: {SetKey}", CacheKeys.ProductSet);
        }

        return result;
    }
}
