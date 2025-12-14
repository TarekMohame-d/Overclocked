using Overclocked.Application.Abstractions.Caching;
using Overclocked.Application.Abstractions.Messaging;

namespace Overclocked.Application.Brand.Commands.CreateBrand;

public record CreateBrandCommand : ICommand, ICacheInvalidatorCommand
{
    public required string Name { get; init; }
    public required string ImageUrl { get; init; }

    public string[] CacheKeys =>
    [
        Common.Constants.CacheKeys.AllBrands
    ];

    public string? CacheSetKey => null;
}
