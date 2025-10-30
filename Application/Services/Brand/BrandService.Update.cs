using System.Net;
using Application.Common.Results;
using Application.Features.Brand.Mapping;
using Application.Services.Brand.DTOs.Request;

namespace Application.Services.Brand;

public sealed partial class BrandService
{
    public async Task<Result> UpdateBrandAsync(UpdateBrandRequest request, CancellationToken cancellationToken)
    {
        var brand = await _brandRepository.GetByIdAsync([request.Id], cancellationToken);

        if (brand is null)
            return Result.Failure(Errors.BrandNotFound, HttpStatusCode.NotFound);

        if (brand.Name != request.Name)
        {
            bool exist = await _brandRepository
                .AnyAsync(x => x.NormalizedName == request.Name.ToUpper(), cancellationToken);

            if (exist)
                return Result.Failure(Errors.BrandNameAlreadyExists, HttpStatusCode.Conflict);
        }

        // Delete old image
        if (brand.Image != request.ImageUrl)
            await _fileStorageService.DeleteFileAsync(brand.Image, cancellationToken);


        brand.UpdateFrom(request);

        _brandRepository.Update(brand);

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success();
    }
}
