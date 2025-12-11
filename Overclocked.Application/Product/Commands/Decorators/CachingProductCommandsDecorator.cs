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
        Result result = await inner.CreateProductCommandHandler(command, cancellationToken);

        if(result.IsSuccess)
        {
            logger.LogInformation("Removing cache for Product set: {SetKey}", CacheKeys.ProductSet);
            await cacheService.RemoveKeysInSetAsync(CacheKeys.ProductSet);
            logger.LogInformation("Removed cache for Product set: {SetKey}", CacheKeys.ProductSet);
        }

        return result;
    }

    public async Task<Result> UpdateProductCommandHandler(
        UpdateProductCommand command,
        CancellationToken cancellationToken)
    {
        Result result = await inner.UpdateProductCommandHandler(command, cancellationToken);

        if(result.IsSuccess)
        {
            var key = CacheKeys.Product(command.Id.ToString());

            logger.LogInformation("Removing cache for Product key: {CacheKey}", key);
            await cacheService.RemoveAsync(key, cancellationToken);
            logger.LogInformation("Removed cache for Product key: {CacheKey}", key);

            logger.LogInformation("Removing cache for Products key: {CacheKey}", CacheKeys.ProductSet);
            await cacheService.RemoveAsync(CacheKeys.ProductSet, cancellationToken);
            logger.LogInformation("Removed cache for Products key: {CacheKey}", CacheKeys.ProductSet);
        }

        return result;
    }

    public async Task<Result> DeleteProductCommandHandler(
        DeleteProductCommand command,
        CancellationToken cancellationToken)
    {
        Result result = await inner.DeleteProductCommandHandler(command, cancellationToken);

        if(result.IsSuccess)
        {
            var key = CacheKeys.Product(command.Id.ToString());

            logger.LogInformation("Removing cache for Product key: {CacheKey}", key);
            await cacheService.RemoveAsync(key, cancellationToken);
            logger.LogInformation("Removed cache for Product key: {CacheKey}", key);

            logger.LogInformation("Removing cache for Products key: {CacheKey}", CacheKeys.ProductSet);
            await cacheService.RemoveAsync(CacheKeys.ProductSet, cancellationToken);
            logger.LogInformation("Removed cache for Products key: {CacheKey}", CacheKeys.ProductSet);
        }

        return result;
    }
}
