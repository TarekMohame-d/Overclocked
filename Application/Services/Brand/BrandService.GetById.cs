using System.Net;
using Application.Common.Results;
using Application.Common.Results.PredefinedErrors;
using Application.Services.Brand.DTOs.Request;
using Application.Services.Brand.DTOs.Response;
using Application.Services.Brand.Mapping;

namespace Application.Services.Brand;

public sealed partial class BrandService
{
    public async Task<Result<BrandResponse>> GetBrandByIdAsync(
        GetBrandByIdRequest request,
        CancellationToken cancellationToken)
    {
        Domain.Entities.Brand? brand = await brandRepository
            .SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);

        return brand?.ToDto() ?? Result<BrandResponse>.Failure(Errors.BrandNotFound, HttpStatusCode.NotFound);
    }
}
