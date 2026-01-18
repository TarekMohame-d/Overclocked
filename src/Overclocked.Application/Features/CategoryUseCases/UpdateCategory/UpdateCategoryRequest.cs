using Overclocked.Application.Abstractions.Caching;
using Overclocked.Application.Abstractions.Messaging;

namespace Overclocked.Application.Features.CategoryUseCases.UpdateCategory;

public record UpdateCategoryRequest : IRequest, ICacheInvalidatorRequest
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string ImageUrl { get; init; }

    public string[] CacheKeys => [Common.Constants.CacheKeys.Category(Id.ToString()), Common.Constants.CacheKeys.AllCategories];
    public string? CacheSetKey => null;
}
