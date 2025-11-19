using Application.Abstraction.Messaging;
using Application.Common.Constants;

namespace Application.Services.Product.DTOs.Request;

public record GetProductByIdRequest : ICachedRequest
{
    public required Guid Id { get; init; }
    public string CacheKey => CacheKeys.Product(Id.ToString());
    public string? CacheSetKey => null;
    public TimeSpan SlidingExpiration => TimeSpan.FromMinutes(5);
}
