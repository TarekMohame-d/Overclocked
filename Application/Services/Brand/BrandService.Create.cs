using System.Net;
using Application.Common.Results;
using Application.Services.Brand.DTOs.Request;
using Application.Services.Brand.Mapping;

namespace Application.Services.Brand;

public sealed partial class BrandService
{
    public async Task<Result> CreateBrandAsync(CreateBrandRequest request, CancellationToken cancellationToken)
    {
        Domain.Entities.Brand brand = request.ToEntity();

        await brandRepository.AddAsync(brand, cancellationToken);

        await unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success(HttpStatusCode.Created);
    }
}
