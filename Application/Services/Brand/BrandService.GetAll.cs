using Application.Common.Results;
using Application.Features.Brand.Mapping;
using Application.Services.Brand.DTOs.Request;
using Application.Services.Brand.DTOs.Response;

namespace Application.Services.Brand;

public sealed partial class BrandService
{
    public async Task<Result<IEnumerable<BrandListResponse>>> GetAllBrandsAsync(GetAllBrandsRequest request, CancellationToken cancellationToken)
    {
        IEnumerable<BrandListResponse> result = [];
        var brands = await _brandRepository.GetAllAsync(cancellationToken: cancellationToken);

        if (brands.Any())
            result = brands.ToDto();

        return Result<IEnumerable<BrandListResponse>>.Success(result);
    }
}
