using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Features.BrandUseCases.DTOs.Responses;
using Overclocked.Application.Features.BrandUseCases.Mapping;
using Overclocked.Domain.BrandAggregate;
using Overclocked.SharedKernel;

namespace Overclocked.Application.Features.BrandUseCases.GetAllBrands;

public class GetAllBrandsRequestHandler(IBrandReadRepository brandRepository)
    : IRequestHandler<GetAllBrandsRequest, IEnumerable<BrandListResponse>>
{
    public async Task<Result<IEnumerable<BrandListResponse>>> Handle(GetAllBrandsRequest request, CancellationToken ct)
    {
        IEnumerable<Brand> result = await brandRepository.GetAllAsync(ct);

        return Result.Success(result.ToDto());
    }
}
