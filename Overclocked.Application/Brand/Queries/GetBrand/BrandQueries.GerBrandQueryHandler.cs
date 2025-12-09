using System.Net;
using Overclocked.Application.Brand.Mapping;
using Overclocked.Application.Brand.Queries.GetBrand;
using Overclocked.Application.Category.Mapping;
using Overclocked.Contracts.Brand;
using Overclocked.Domain.Common.Errors;
using Overclocked.Domain.Common.Results;

namespace Overclocked.Application.Brand.Queries;

public sealed partial class BrandQueries
{
    public async Task<Result<BrandResponse>> GetBrandQueryHandler(
        GetBrandQuery query,
        CancellationToken cancellationToken)
    {
        Domain.BrandAggregate.Brand? brand = await brandRepository.GetBrandByIdAsync(query.Id, cancellationToken);

        return brand is null
            ? Result<BrandResponse>.Failure(BrandErrors.BrandNotFound(query.Id), HttpStatusCode.NotFound)
            : Result<BrandResponse>.Success(brand.ToDto());
    }
}
