using Microsoft.Extensions.Logging;
using Overclocked.Application.Abstraction.Services;
using Overclocked.Application.Category.Commands.CreateCategory;
using Overclocked.Application.Category.Commands.DeleteCategory;
using Overclocked.Application.Category.Commands.UpdateCategory;
using Overclocked.Application.Common.Constants;
using Overclocked.Domain.Common.Results;

namespace Overclocked.Application.Category.Commands.Decorators;

public class CachingCategoryCommandsDecorator(
    ICategoryCommands inner,
    ICacheService cacheService,
    ILogger<CachingCategoryCommandsDecorator> logger) : ICategoryCommands
{
    public async Task<Result> CreateCategoryCommandHandler(
        CreateCategoryCommand command,
        CancellationToken cancellationToken)
    {
        Result result = await inner.CreateCategoryCommandHandler(command, cancellationToken);

        if(result.IsSuccess)
        {
            logger.LogInformation("Removing cache for Categories key: {CacheKey}", CacheKeys.AllCategories);
            await cacheService.RemoveAsync(CacheKeys.AllCategories, cancellationToken);
            logger.LogInformation("Removed cache for Categories key: {CacheKey}", CacheKeys.AllCategories);
        }

        return result;
    }

    public async Task<Result> UpdateCategoryCommandHandler(
        UpdateCategoryCommand command,
        CancellationToken cancellationToken)
    {
        Result result = await inner.UpdateCategoryCommandHandler(command, cancellationToken);

        if(result.IsSuccess)
        {
            var key = CacheKeys.Category(command.Id.ToString());

            logger.LogInformation("Removing cache for Category key: {CacheKey}", key);
            await cacheService.RemoveAsync(key, cancellationToken);
            logger.LogInformation("Removed cache for Category key: {CacheKey}", key);

            logger.LogInformation("Removing cache for Categories key: {CacheKey}", CacheKeys.AllCategories);
            await cacheService.RemoveAsync(CacheKeys.AllCategories, cancellationToken);
            logger.LogInformation("Removed cache for Categories key: {CacheKey}", CacheKeys.AllCategories);
        }

        return result;
    }

    public async Task<Result> DeleteCategoryCommandHandler(
        DeleteCategoryCommand command,
        CancellationToken cancellationToken)
    {
        Result result = await inner.DeleteCategoryCommandHandler(command, cancellationToken);

        if(result.IsSuccess)
        {
            var key = CacheKeys.Category(command.Id.ToString());

            logger.LogInformation("Removing cache for Category key: {CacheKey}", key);
            await cacheService.RemoveAsync(key, cancellationToken);
            logger.LogInformation("Removed cache for Category key: {CacheKey}", key);

            logger.LogInformation("Removing cache for Categories key: {CacheKey}", CacheKeys.AllCategories);
            await cacheService.RemoveAsync(CacheKeys.AllCategories, cancellationToken);
            logger.LogInformation("Removed cache for Categories key: {CacheKey}", CacheKeys.AllCategories);
        }

        return result;
    }
}
