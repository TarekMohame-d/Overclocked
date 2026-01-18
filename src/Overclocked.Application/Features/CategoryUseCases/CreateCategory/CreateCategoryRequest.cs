using Overclocked.Application.Abstractions.Caching;
using Overclocked.Application.Abstractions.Messaging;

namespace Overclocked.Application.Features.CategoryUseCases.CreateCategory;

public record CreateCategoryRequest : IRequest<Guid>, ICacheInvalidatorRequest
{
    public required string Name { get; init; }
    public required string ImageUrl { get; init; }

    public string[] CacheKeys => [Common.Constants.CacheKeys.AllCategories];
    public string? CacheSetKey => null;
}
