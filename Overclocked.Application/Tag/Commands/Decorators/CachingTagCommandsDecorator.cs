using Microsoft.Extensions.Logging;
using Overclocked.Application.Abstraction.Services;
using Overclocked.Application.Common.Constants;
using Overclocked.Application.Tag.Commands.CreateTag;
using Overclocked.Application.Tag.Commands.DeleteTag;
using Overclocked.Application.Tag.Commands.UpdateTag;
using Overclocked.Domain.Common.Results;

namespace Overclocked.Application.Tag.Commands.Decorators;

public class CachingTagCommandsDecorator(
    ITagCommands inner,
    ICacheService cacheService,
    ILogger<CachingTagCommandsDecorator> logger) : ITagCommands
{
    public async Task<Result> CreateTagCommandHandler(CreateTagCommand command, CancellationToken cancellationToken)
    {
        return await RemoveCache(() => inner.CreateTagCommandHandler(command, cancellationToken));
    }

    public async Task<Result> UpdateTagCommandHandler(UpdateTagCommand command, CancellationToken cancellationToken)
    {
        return await RemoveCache(() => inner.UpdateTagCommandHandler(command, cancellationToken));
    }

    public async Task<Result> DeleteTagCommandHandler(DeleteTagCommand command, CancellationToken cancellationToken)
    {
        return await RemoveCache(() => inner.DeleteTagCommandHandler(command, cancellationToken));
    }

    private async Task<TResult> RemoveCache<TResult>(Func<Task<TResult>> action)
        where TResult : Result
    {
        TResult result = await action();

        if(result.IsSuccess)
        {
            logger.LogInformation("Removing cache for Tags set: {SetKey}", CacheKeys.TagSet);
            await cacheService.RemoveKeysInSetAsync(CacheKeys.TagSet);
            logger.LogInformation("Removed cache for Tags set: {SetKey}", CacheKeys.TagSet);
        }

        return result;
    }
}
