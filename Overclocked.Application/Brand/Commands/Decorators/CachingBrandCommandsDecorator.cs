using Microsoft.Extensions.Logging;
using Overclocked.Application.Abstraction.Services;
using Overclocked.Application.Brand.Commands.CreateBrand;
using Overclocked.Application.Brand.Commands.DeleteBrand;
using Overclocked.Application.Brand.Commands.UpdateBrand;
using Overclocked.Application.Common.Constants;
using Overclocked.Domain.Common.Results;

namespace Overclocked.Application.Brand.Commands.Decorators;

public class CachingBrandCommandsDecorator(
    IBrandCommands inner,
    ICacheService cacheService,
    ILogger<CachingBrandCommandsDecorator> logger) : IBrandCommands
{
    public async Task<Result> CreateBrandCommandHandler(CreateBrandCommand command, CancellationToken cancellationToken)
    {
        Result result = await inner.CreateBrandCommandHandler(command, cancellationToken);

        if(result.IsSuccess)
        {
            logger.LogInformation("Removing cache for Brands key: {CacheKey}", CacheKeys.AllBrands);
            await cacheService.RemoveAsync(CacheKeys.AllBrands, cancellationToken);
            logger.LogInformation("Removed cache for Brands key: {CacheKey}", CacheKeys.AllBrands);
        }

        return result;
    }

    public async Task<Result> UpdateBrandCommandHandler(UpdateBrandCommand command, CancellationToken cancellationToken)
    {
        Result result = await inner.UpdateBrandCommandHandler(command, cancellationToken);

        if(result.IsSuccess)
        {
            var key = CacheKeys.Brand(command.Id.ToString());

            logger.LogInformation("Removing cache for Brand key: {CacheKey}", key);
            await cacheService.RemoveAsync(key, cancellationToken);
            logger.LogInformation("Removed cache for Brand key: {CacheKey}", key);

            logger.LogInformation("Removing cache for Brands key: {CacheKey}", CacheKeys.AllBrands);
            await cacheService.RemoveAsync(CacheKeys.AllBrands, cancellationToken);
            logger.LogInformation("Removed cache for Brands key: {CacheKey}", CacheKeys.AllBrands);
        }

        return result;
    }

    public async Task<Result> DeleteBrandCommandHandler(DeleteBrandCommand command, CancellationToken cancellationToken)
    {
        Result result = await inner.DeleteBrandCommandHandler(command, cancellationToken);

        if(result.IsSuccess)
        {
            var key = CacheKeys.Brand(command.Id.ToString());

            logger.LogInformation("Removing cache for Brand key: {CacheKey}", key);
            await cacheService.RemoveAsync(key, cancellationToken);
            logger.LogInformation("Removed cache for Brand key: {CacheKey}", key);

            logger.LogInformation("Removing cache for Brands key: {CacheKey}", CacheKeys.AllBrands);
            await cacheService.RemoveAsync(CacheKeys.AllBrands, cancellationToken);
            logger.LogInformation("Removed cache for Brands key: {CacheKey}", CacheKeys.AllBrands);
        }

        return result;
    }
}
