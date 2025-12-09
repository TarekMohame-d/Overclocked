using Overclocked.Application.Brand.Mapping;
using Overclocked.Application.Brand.Queries.GetAllBrands;
using Overclocked.Application.Category.Mapping;
using Overclocked.Contracts.Brand;
using Overclocked.Domain.Common.Results;

namespace Overclocked.Application.Brand.Queries;

public sealed partial class BrandQueries
{
    public async Task<Result<IEnumerable<BrandListResponse>>> GetBrandListQueryHandler(
        GetBrandListQuery query,
        CancellationToken cancellationToken)
    {
        IEnumerable<Domain.BrandAggregate.Brand> result = await brandRepository.GetBrandListAsync(cancellationToken);

        return Result<IEnumerable<BrandListResponse>>.Success(result.ToDto());
    }
}
