using Overclocked.Application.Abstractions.Caching;
using Overclocked.Application.Abstractions.Messaging;

namespace Overclocked.Application.Tag.Commands.DeleteTag;

public record DeleteTagCommand : ICommand, ICacheInvalidatorCommand
{
    public required Guid Id { get; init; }

    public string[] CacheKeys => [];

    public string? CacheSetKey => Common.Constants.CacheKeys.TagSet;
}
