using System.Net;
using Application.Common.Results;
using Application.Services.Category.DTOs.Request;

namespace Application.Services.Category;

public sealed partial class CategoryService
{
    public async Task<Result> DeleteCategoryAsync(DeleteCategoryRequest request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync([request.Id], cancellationToken);

        if (category is null)
            return Result.Failure(Errors.CategoryNotFound, HttpStatusCode.NotFound);

        _categoryRepository.Delete(category);

        await _fileStorageService.DeleteFileAsync(category.Image, cancellationToken);

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success();
    }
}
