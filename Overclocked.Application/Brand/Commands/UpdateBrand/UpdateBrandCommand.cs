using Overclocked.Application.Abstractions.Caching;
using Overclocked.Application.Abstractions.Messaging;

namespace Overclocked.Application.Brand.Commands.UpdateBrand;

public record UpdateBrandCommand : ICommand, ICacheInvalidatorCommand
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string ImageUrl { get; init; }
    public string[] CacheKeys =>
    [
        Common.Constants.CacheKeys.Brand(Id.ToString()),
        Common.Constants.CacheKeys.AllCategories
    ];
    public string? CacheSetKey => null;
}
