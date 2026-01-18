using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Domain.BrandAggregate;
using Overclocked.Domain.Common.Shared.ValueObjects.Image;
using Overclocked.SharedKernel;

namespace Overclocked.Application.Features.BrandUseCases.CreateBrand;

public class CreateBrandRequestHandler(IBrandRepository brandRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<CreateBrandRequest, Guid>
{
    public async Task<Result<Guid>> Handle(CreateBrandRequest request, CancellationToken ct)
    {
        if (await brandRepository.NameExistsAsync(request.Name, ct))
            return Result.Failure<Guid>(BrandErrors.BrandNameAlreadyExists);

        Result<Image> imageResult = Image.Create(request.ImageUrl);

        if (imageResult.IsFailure)
            return Result.Failure<Guid>(imageResult.Error);

        Result<Brand> brandResult = Brand.Create(request.Name, imageResult.Value);

        if (brandResult.IsFailure)
            return Result.Failure<Guid>(brandResult.Error);

        Brand brand = brandResult.Value;
        brandRepository.Add(brand);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(brand.Id.Value);
    }
}
