using System.Net;
using Application.Common.Results;
using Application.Services.Brand.DTOs.Request;
using Application.Services.Tag.DTOs.Request;

namespace Application.Services.Brand;

public sealed partial class BrandService
{
    public async Task<Result> DeleteBrandAsync(DeleteBrandRequest request, CancellationToken cancellationToken)
    {
        var brand = await _brandRepository.GetByIdAsync([request.Id], cancellationToken);

        if (brand is null)
            return Result.Failure(Errors.BrandNotFound, HttpStatusCode.NotFound);

        _brandRepository.Delete(brand);

        await _fileStorageService.DeleteFileAsync(brand.Image, cancellationToken);

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success();
    }
}
