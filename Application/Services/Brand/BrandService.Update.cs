using System.Net;
using Application.Common.Results;
using Application.Services.Brand.DTOs.Request;
using Application.Services.Brand.Events;
using Application.Services.Brand.Mapping;

namespace Application.Services.Brand;

public sealed partial class BrandService
{
    public async Task<Result> UpdateBrandAsync(UpdateBrandRequest request, CancellationToken cancellationToken)
    {
        Domain.Entities.Brand? brand = await brandRepository.GetByIdAsync([request.Id], cancellationToken);

        if (brand is null)
            return Result.Failure(Errors.BrandNotFound, HttpStatusCode.NotFound);

        if (brand.Name != request.Name)
        {
            var exist = await brandRepository
                .AnyAsync(x => x.NormalizedName == request.Name.ToUpper(), cancellationToken);

            if (exist)
                return Result.Failure(Errors.BrandNameAlreadyExists, HttpStatusCode.Conflict);
        }

        // Delete old image
        if (brand.Image != request.ImageUrl)
        {
            BrandUpdatedEvent brandUpdatedEvent = new(brand.Image);
            await eventDispatcher.DispatchAsync(brandUpdatedEvent, cancellationToken);
        }

        brand.UpdateFrom(request);

        brandRepository.Update(brand);

        await unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success();
    }
}
