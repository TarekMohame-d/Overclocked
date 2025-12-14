using Overclocked.Application.Abstractions.Caching;
using Overclocked.Application.Abstractions.Messaging;

namespace Overclocked.Application.Product.Commands.DeleteProduct;

public record DeleteProductCommand : ICommand, ICacheInvalidatorCommand
{
    public required Guid Id { get; init; }

    public string[] CacheKeys =>
    [
        Common.Constants.CacheKeys.Product(Id.ToString())
    ];

    public string? CacheSetKey => Common.Constants.CacheKeys.ProductSet;
}
