using Application.Abstraction.Messaging;
using Application.Common.Constants;
using Application.Common.Results;

namespace Application.Features.Brand.Queries.GetAllBrands;

public record GetAllBrandsQuery : ICachedQuery<Result<IEnumerable<BrandListDto>>>
{
    public string CacheKey => CacheKeys.AllBrands;
    public string? CacheSetKey => null;
    public bool BypassCache => false;

}
