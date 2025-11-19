using System.Net;
using Application.Common.Results;
using Application.Common.Results.PredefinedErrors;
using Application.Services.Brand.DTOs.Request;
using Application.Services.Brand.Events;

namespace Application.Services.Brand;

public sealed partial class BrandService
{
    public async Task<Result> DeleteBrandAsync(DeleteBrandRequest request, CancellationToken cancellationToken)
    {
        Domain.Entities.Brand? brand = await brandRepository.GetByIdAsync([request.Id], cancellationToken);

        if(brand is null)
        {
            return Result.Failure(Errors.BrandNotFound, HttpStatusCode.NotFound);
        }

        brandRepository.Delete(brand);

        BrandDeletedEvent brandDeletedEvent = new(brand.Image);
        await eventDispatcher.DispatchAsync(brandDeletedEvent, cancellationToken);

        await unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success();
    }
}
