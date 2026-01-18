using Overclocked.Application.Abstractions.Caching;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Common.Constants;
using Overclocked.Application.Features.CategoryUseCases.DTOs.Responses;

namespace Overclocked.Application.Features.CategoryUseCases.GetAllCategories;

public record GetAllCategoriesRequest : IRequest<IEnumerable<CategoryListResponse>>, ICachedRequest
{
    public string CacheKey => CacheKeys.AllCategories;
    public string? CacheSetKey => null;
    public TimeSpan Expiration => TimeSpan.FromMinutes(5);
}
