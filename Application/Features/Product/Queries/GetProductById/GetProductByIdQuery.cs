using Application.Abstraction.Messaging;
using Application.Common.Constants;
using Application.Common.Results;

namespace Application.Features.Product.Queries.GetProductById;

public record GetProductByIdQuery : ICachedRequest<Result<ProductDto>>
{
    public Guid Id { get; init; }
    public string CacheKey => CacheKeys.Product(Id.ToString());
    public bool BypassCache => false;
    public string? CacheSetKey => null;
}
