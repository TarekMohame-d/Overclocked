using Application.Abstraction.Messaging;
using Application.Common.Constants;

namespace Application.Services.Category.DTOs.Request;

public record GetAllCategoriesRequest : ICachedRequest
{
    public string CacheKey => CacheKeys.AllCategories;
    public string? CacheSetKey => null;
    public TimeSpan SlidingExpiration => TimeSpan.FromMinutes(5);
}
