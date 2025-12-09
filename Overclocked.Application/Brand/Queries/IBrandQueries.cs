using Overclocked.Application.Brand.Queries.GetAllBrands;
using Overclocked.Application.Brand.Queries.GetBrand;
using Overclocked.Contracts.Brand;
using Overclocked.Domain.Common.Results;

namespace Overclocked.Application.Brand.Queries;

public interface IBrandQueries
{
    Task<Result<BrandResponse>> GetBrandQueryHandler(GetBrandQuery query, CancellationToken cancellationToken);
    Task<Result<IEnumerable<BrandListResponse>>> GetBrandListQueryHandler(
        GetBrandListQuery query,
        CancellationToken cancellationToken);
}
