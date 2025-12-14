using Overclocked.Application.Abstractions.Caching;
using Overclocked.Application.Abstractions.Messaging;

namespace Overclocked.Application.Brand.Commands.DeleteBrand;

public record DeleteBrandCommand : ICommand, ICacheInvalidatorCommand
{
    public required Guid Id { get; init; }
    public string[] CacheKeys =>
    [
        Common.Constants.CacheKeys.Brand(Id.ToString()),
        Common.Constants.CacheKeys.AllBrands
    ];

    public string? CacheSetKey => null;
}
