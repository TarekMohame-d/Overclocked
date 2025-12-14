using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Brand.Mapping;
using Overclocked.Application.Category.Mapping;
using Overclocked.Contracts.Brand;
using Overclocked.Domain.Common.Results;

namespace Overclocked.Application.Brand.Queries.GetAllBrands;

public class GetAllBrandsQueryHandler(IBrandRepository brandRepository)
    : IQueryHandler<GetAllBrandsQuery, IEnumerable<BrandListResponse>>
{
    private readonly IBrandRepository _brandRepository = brandRepository;

    public async Task<Result<IEnumerable<BrandListResponse>>> Handle(
        GetAllBrandsQuery query,
        CancellationToken cancellationToken)
    {
        IEnumerable<Domain.BrandAggregate.Brand> result = await _brandRepository.GetAllAsync(cancellationToken);

        return Result.Success(result.ToDto());
    }
}
