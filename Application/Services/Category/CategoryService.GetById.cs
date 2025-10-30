using System.Net;
using Application.Common.Results;
using Application.Features.Category.Mapping;
using Application.Services.Category.DTOs.Request;
using Application.Services.Category.DTOs.Response;

namespace Application.Services.Category;

public sealed partial class CategoryService
{
    public async Task<Result<CategoryResponse>> GetCategoryByIdAsync(GetCategoryByIdRequest request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync([request.Id], cancellationToken);

        if (category is null)
            return Result<CategoryResponse>.Failure(
                Errors.CategoryNotFound,
                HttpStatusCode.NotFound);

        return category.ToDto();
    }
}
