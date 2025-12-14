using Overclocked.Application.Abstractions.Caching;
using Overclocked.Application.Abstractions.Messaging;

namespace Overclocked.Application.Tag.Commands.CreateTag;

public record CreateTagCommand : ICommand, ICacheInvalidatorCommand
{
    public required string Name { get; init; }

    public string[] CacheKeys => [];

    public string? CacheSetKey => Common.Constants.CacheKeys.TagSet;
}
