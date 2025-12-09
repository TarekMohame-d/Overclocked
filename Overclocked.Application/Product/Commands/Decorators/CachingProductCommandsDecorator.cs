using Microsoft.Extensions.Logging;
using Overclocked.Application.Abstraction.Services;
using Overclocked.Application.Common.Constants;
using Overclocked.Application.Product.Commands.CreateProduct;
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
            logger.LogInformation("Removing cache for Brands key: {CacheKey}", CacheKeys.AllBrands);
            await cacheService.RemoveAsync(CacheKeys.AllBrands, cancellationToken);
            logger.LogInformation("Removed cache for Brands key: {CacheKey}", CacheKeys.AllBrands);
        }

        return result;
    }
}
