using System.Net;
using Application.Common.Results;
using Application.Common.Results.PredefinedErrors;
using Application.Services.Brand.DTOs.Request;
using Application.Services.Brand.Events;
using Application.Services.Brand.Mapping;

namespace Application.Services.Brand;

public sealed partial class BrandService
{
    public async Task<Result> UpdateBrandAsync(UpdateBrandRequest request, CancellationToken cancellationToken)
    {
        Domain.Entities.Brand? brand = await brandRepository
            .SingleOrDefaultAsync(x => x.Id == request.Id, asNoTracking: false, cancellationToken);

        if(brand is null)
            return Result.Failure(Errors.BrandNotFound, HttpStatusCode.NotFound);

        var oldImageUrl = brand.Image;

        if(brand.Name != request.Name)
        {
            var exist = await brandRepository
                .AnyAsync(x => x.NormalizedName == request.Name.ToUpper(), cancellationToken);

            if(exist)
                return Result.Failure(Errors.BrandNameAlreadyExists, HttpStatusCode.Conflict);
        }

        brand.UpdateFrom(request);

        await unitOfWork.CompleteAsync(cancellationToken);

        // Delete old image if new image is different
        if(oldImageUrl != request.ImageUrl)
        {
            BrandUpdatedEvent brandUpdatedEvent = new(brand.Image);
            await eventDispatcher.DispatchAsync(brandUpdatedEvent, cancellationToken);
        }

        return Result.Success();
    }
}
