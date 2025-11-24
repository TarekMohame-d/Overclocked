using System.Net;
using Application.Common.Results;
using Application.Common.Results.PredefinedErrors;
using Application.Services.Brand.Events;

namespace Application.Services.Brand;

public sealed partial class BrandService
{
    public async Task<Result> DeleteBrandAsync(Guid brandId, CancellationToken cancellationToken)
    {
        Domain.Entities.Brand? brand = await brandRepository.GetByIdAsync([brandId], cancellationToken);

        if(brand is null)
            return Result.Failure(Errors.BrandNotFound, HttpStatusCode.NotFound);

        brandRepository.Delete(brand);

        await unitOfWork.CompleteAsync(cancellationToken);

        BrandDeletedEvent brandDeletedEvent = new(brand.Image);
        await eventDispatcher.DispatchAsync(brandDeletedEvent, cancellationToken);

        return Result.Success();
    }
}
