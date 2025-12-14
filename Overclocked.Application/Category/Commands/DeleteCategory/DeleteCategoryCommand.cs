using Overclocked.Application.Abstractions.Caching;
using Overclocked.Application.Abstractions.Messaging;

namespace Overclocked.Application.Category.Commands.DeleteCategory;

public record DeleteCategoryCommand : ICommand, ICacheInvalidatorCommand
{
    public required Guid Id { get; init; }
    public string[] CacheKeys =>
    [
        Common.Constants.CacheKeys.Category(Id.ToString()),
        Common.Constants.CacheKeys.AllCategories
    ];

    public string? CacheSetKey => null;
}
