using Overclocked.Application.Abstractions.Caching;
using Overclocked.Application.Abstractions.Messaging;

namespace Overclocked.Application.Category.Commands.CreateCategory;

public record CreateCategoryCommand : ICommand, ICacheInvalidatorCommand
{
    public required string Name { get; init; }
    public required string ImageUrl { get; init; }

    public string[] CacheKeys =>
    [
        Common.Constants.CacheKeys.AllCategories
    ];

    public string? CacheSetKey => null;
}
