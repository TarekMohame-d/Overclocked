using Application.Abstraction.Messaging;
using Application.Common.Results;
using Application.Features.Brand.Mapping;
using Domain.Repositories;

namespace Application.Features.Brand.Queries.GetAllBrands;

public class GetAllBrandsQueryHandler : IQueryHandler<GetAllBrandsQuery, Result<IEnumerable<BrandListDto>>>
{
    private readonly IBrandRepository _brandRepository;

    public GetAllBrandsQueryHandler(IBrandRepository brandRepository)
    {
        _brandRepository = brandRepository;
    }

    public async Task<Result<IEnumerable<BrandListDto>>> Handle(GetAllBrandsQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<BrandListDto> result = [];
        var brands = await _brandRepository.GetAllAsync(cancellationToken: cancellationToken);

        if (brands.Any())
            result = brands.ToDto();

        return Result<IEnumerable<BrandListDto>>.Success(result);
    }
}
