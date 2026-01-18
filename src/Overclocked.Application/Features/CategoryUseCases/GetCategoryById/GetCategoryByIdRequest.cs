using Overclocked.Application.Abstractions.Caching;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Common.Constants;
using Overclocked.Application.Features.CategoryUseCases.DTOs.Responses;

namespace Overclocked.Application.Features.CategoryUseCases.GetCategoryById;

public record GetCategoryByIdRequest : IRequest<CategoryResponse>, ICachedRequest
{
    public required Guid Id { get; init; }

    public string CacheKey => CacheKeys.Category(Id.ToString());
    public string? CacheSetKey => null;
    public TimeSpan Expiration => TimeSpan.FromMinutes(5);
}
