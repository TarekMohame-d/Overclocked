using Application.Abstraction.Messaging;
using Application.Common.Constants;

namespace Application.Services.Category.DTOs.Request;

public record GetCategoryByIdRequest : ICachedRequest
{
    public Guid Id { get; init; }
    public string CacheKey => CacheKeys.Category(Id.ToString());
    public string? CacheSetKey => null;
    public TimeSpan SlidingExpiration => TimeSpan.FromMinutes(5);
}
