using Overclocked.Application.Abstractions.Caching;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Common.Constants;
using Overclocked.Application.Features.ProductUseCases.DTOs.Responses;

namespace Overclocked.Application.Features.ProductUseCases.GetProductById;

public record GetProductByIdRequest : IRequest<ProductResponse>, ICachedRequest
{
    public required Guid Id { get; init; }

    public string CacheKey => CacheKeys.Product(Id.ToString());
    public string? CacheSetKey => null;
    public TimeSpan Expiration => TimeSpan.FromMinutes(5);
}
