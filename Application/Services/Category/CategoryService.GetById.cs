using System.Net;
using Application.Common.Results;
using Application.Services.Category.DTOs.Request;
using Application.Services.Category.DTOs.Response;
using Application.Services.Category.Mapping;

namespace Application.Services.Category;

public sealed partial class CategoryService
{
    public async Task<Result<CategoryResponse>> GetCategoryByIdAsync(GetCategoryByIdRequest request,
        CancellationToken cancellationToken)
    {
        Domain.Entities.Category? category = await categoryRepository.GetByIdAsync([request.Id], cancellationToken);

        return category?.ToDto() ?? Result<CategoryResponse>.Failure(
            Errors.CategoryNotFound,
            HttpStatusCode.NotFound);
    }
}
