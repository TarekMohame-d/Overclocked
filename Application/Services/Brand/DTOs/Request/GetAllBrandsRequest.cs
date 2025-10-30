using Application.Abstraction.Messaging;
using Application.Common.Constants;

namespace Application.Services.Brand.DTOs.Request;

public record GetAllBrandsRequest : ICachedRequest
{
    public string CacheKey => CacheKeys.AllBrands;
    public string? CacheSetKey => null;
    public TimeSpan SlidingExpiration => TimeSpan.FromMinutes(5);
}

