using Application.Abstraction.Messaging;
using Application.Common.Constants;
using Application.Common.Results;
namespace Application.Features.Brand.Queries.GetBrandById;

public record GetBrandByIdQuery : ICachedRequest<Result<BrandDto>>
{
    public Guid Id { get; init; }
    public string CacheKey => CacheKeys.Brand(Id.ToString());
    public string? CacheSetKey => null;
    public bool BypassCache => false;
}
