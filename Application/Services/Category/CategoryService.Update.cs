using System.Net;
using Application.Common.Results;
using Application.Features.Category.Mapping;
using Application.Services.Category.DTOs.Request;

namespace Application.Services.Category;

public sealed partial class CategoryService
{
    public async Task<Result> UpdateCategoryAsync(UpdateCategoryRequest request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync([request.Id], cancellationToken);

        if (category is null)
            return Result.Failure(Errors.CategoryNotFound, HttpStatusCode.NotFound);

        if (category.Name != request.Name)
        {
            bool exist = await _categoryRepository
                .AnyAsync(x => x.NormalizedName == request.Name.ToUpper(), cancellationToken);

            if (exist)
                return Result.Failure(Errors.CategoryNameAlreadyExists, HttpStatusCode.Conflict);
        }

        // Delete old image
        if (category.Image != request.ImageUrl)
            await _fileStorageService.DeleteFileAsync(category.Image, cancellationToken);


        category.UpdateFrom(request);

        _categoryRepository.Update(category);

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success();
    }
}
