using Overclocked.Application.Abstractions.Caching;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Common.Constants;
using Overclocked.Application.Features.BrandUseCases.DTOs.Responses;

namespace Overclocked.Application.Features.BrandUseCases.GetAllBrands;

public record GetAllBrandsRequest : IRequest<IEnumerable<BrandListResponse>>, ICachedRequest
{
    public string CacheKey => CacheKeys.AllBrands;
    public string? CacheSetKey => null;
    public TimeSpan Expiration => TimeSpan.FromMinutes(5);
}
