using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Brand.Mapping;
using Overclocked.Application.Category.Mapping;
using Overclocked.Contracts.Brand;
using Overclocked.Domain.Common.Errors;
using Overclocked.Domain.Common.Results;

namespace Overclocked.Application.Brand.Queries.GetBrandById;

public sealed partial class GetBrandByIdQueryHandler(IBrandRepository brandRepository)
    : IQueryHandler<GetBrandByIdQuery, BrandResponse>
{
    public async Task<Result<BrandResponse>> Handle(GetBrandByIdQuery query, CancellationToken cancellationToken)
    {
        Domain.BrandAggregate.Brand? brand = await brandRepository.GetByIdAsync(query.Id, cancellationToken);

        return brand is null
            ? Result.Failure<BrandResponse>(BrandErrors.BrandNotFound(query.Id))
            : Result.Success(brand.ToDto());
    }
}
