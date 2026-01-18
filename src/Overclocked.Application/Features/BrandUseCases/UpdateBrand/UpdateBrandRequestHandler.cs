using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Domain.BrandAggregate;
using Overclocked.Domain.BrandAggregate.ValueObjects;
using Overclocked.Domain.Common.Shared.ValueObjects.Image;
using Overclocked.SharedKernel;

namespace Overclocked.Application.Features.BrandUseCases.UpdateBrand;

public class UpdateBrandRequestHandler(IBrandRepository brandRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateBrandRequest>
{
    public async Task<Result> Handle(UpdateBrandRequest request, CancellationToken ct)
    {
        Result<Image> imageResult = Image.Create(request.ImageUrl);

        if (imageResult.IsFailure)
            return Result.Failure<Guid>(imageResult.Error);

        Brand? brand = await brandRepository.GetByIdAsync(BrandId.Create(request.Id), ct);

        if (brand is null)
            return Result.Failure(BrandErrors.BrandNotFound(request.Id));

        if (brand.Name != request.Name)
        {
            var exist = await brandRepository.NameExistsAsync(request.Name, ct);

            if (exist)
                return Result.Failure(BrandErrors.BrandNameAlreadyExists);
        }

        Result result = brand.Update(request.Name, imageResult.Value);

        if (result.IsFailure)
            return Result.Failure(result.Error);

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
