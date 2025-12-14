using Overclocked.Application.Abstractions.Caching;
using Overclocked.Application.Abstractions.Messaging;

namespace Overclocked.Application.Tag.Commands.UpdateTag;

public record UpdateTagCommand : ICommand, ICacheInvalidatorCommand
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }

    public string[] CacheKeys => [];

    public string? CacheSetKey => Common.Constants.CacheKeys.TagSet;
}
