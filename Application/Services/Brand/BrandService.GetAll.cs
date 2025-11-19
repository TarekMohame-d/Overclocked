using Application.Common.Results;
using Application.Services.Brand.DTOs.Request;
using Application.Services.Brand.DTOs.Response;
using Application.Services.Brand.Mapping;

namespace Application.Services.Brand;

public sealed partial class BrandService
{
    public async Task<Result<IEnumerable<BrandListResponse>>> GetAllBrandsAsync(
        GetAllBrandsRequest request,
        CancellationToken cancellationToken
    )
    {
        IEnumerable<BrandListResponse> result = [];
        IEnumerable<Domain.Entities.Brand> brands = await brandRepository.GetAllAsync(
            cancellationToken: cancellationToken
        );

        if(brands.Any())
            result = brands.ToDto();

        return Result<IEnumerable<BrandListResponse>>.Success(result);
    }
}
