using Application.Abstraction.Messaging;
using Application.Common.Constants;

namespace Application.Services.Tag.DTOs.Request;

public record GetTagByIdRequest : ICachedRequest
{
    public required Guid Id { get; init; }
    public string CacheKey => CacheKeys.Tag(Id.ToString());
    public string? CacheSetKey => null;
    public TimeSpan SlidingExpiration => TimeSpan.FromMinutes(5);
}
