using System.Net;
using Application.Common.Results;
using Application.Features.Brand.Mapping;
using Application.Services.Brand.DTOs.Request;
using Application.Services.Brand.DTOs.Response;

namespace Application.Services.Brand;

public sealed partial class BrandService
{
    public async Task<Result<BrandResponse>> GetBrandByIdAsync(GetBrandByIdRequest request, CancellationToken cancellationToken)
    {
        var brand = await _brandRepository.GetByIdAsync([request.Id], cancellationToken);

        if (brand is null)
            return Result<BrandResponse>.Failure(
                Errors.BrandNotFound,
                HttpStatusCode.NotFound);

        return brand.ToDto();
    }
}
