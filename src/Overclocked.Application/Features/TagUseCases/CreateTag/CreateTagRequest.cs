using Overclocked.Application.Abstractions.Caching;
using Overclocked.Application.Abstractions.Messaging;

namespace Overclocked.Application.Features.TagUseCases.CreateTag;

public record CreateTagRequest : IRequest<Guid>, ICacheInvalidatorRequest
{
    public required string Name { get; init; }

    public string[] CacheKeys => [];
    public string? CacheSetKey => Common.Constants.CacheKeys.TagSet;
}
