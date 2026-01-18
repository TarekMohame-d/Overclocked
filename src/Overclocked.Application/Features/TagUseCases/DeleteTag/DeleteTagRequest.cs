using Overclocked.Application.Abstractions.Caching;
using Overclocked.Application.Abstractions.Messaging;

namespace Overclocked.Application.Features.TagUseCases.DeleteTag;

public record DeleteTagRequest : IRequest, ICacheInvalidatorRequest
{
    public required Guid Id { get; init; }

    public string[] CacheKeys => [];
    public string? CacheSetKey => Common.Constants.CacheKeys.TagSet;
}
