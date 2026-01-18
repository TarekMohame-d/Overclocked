using Overclocked.Application.Abstractions.Caching;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Common.Constants;
using Overclocked.Application.Features.BrandUseCases.DTOs.Responses;

namespace Overclocked.Application.Features.BrandUseCases.GetBrandById;

public record GetBrandByIdRequest : IRequest<BrandResponse>, ICachedRequest
{
    public required Guid Id { get; init; }

    public string CacheKey => CacheKeys.Brand(Id.ToString());
    public string? CacheSetKey => null;
    public TimeSpan Expiration => TimeSpan.FromMinutes(5);
}
