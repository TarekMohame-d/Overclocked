using System.Net;
using Application.Common.Results;
using Application.Features.Brand.Mapping;
using Application.Services.Brand.DTOs.Request;
using Application.Services.Tag.DTOs.Request;

namespace Application.Services.Brand;

public sealed partial class BrandService
{
    public async Task<Result> CreateBrandAsync(CreateBrandRequest request, CancellationToken cancellationToken)
    {
        var brand = request.ToEntity();

        await _brandRepository.AddAsync(brand, cancellationToken);

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success(HttpStatusCode.Created);
    }
}
