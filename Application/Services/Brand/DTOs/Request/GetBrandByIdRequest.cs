using Application.Abstraction.Messaging;
using Application.Common.Constants;

namespace Application.Services.Brand.DTOs.Request;

public record GetBrandByIdRequest : ICachedRequest
{
    public required Guid Id { get; init; }
    public string CacheKey => CacheKeys.Brand(Id.ToString());
    public string? CacheSetKey => null;
    public TimeSpan SlidingExpiration => TimeSpan.FromMinutes(5);
}
