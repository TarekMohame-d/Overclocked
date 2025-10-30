using Application.Common.Results;
using Application.Features.Category.Mapping;
using Application.Services.Category.DTOs.Request;
using Application.Services.Category.DTOs.Response;

namespace Application.Services.Category;

public sealed partial class CategoryService
{
    public async Task<Result<IEnumerable<CategoryListResponse>>> GetAllCategoriesAsync(GetAllCategoriesRequest request, CancellationToken cancellationToken)
    {
        IEnumerable<CategoryListResponse> result = [];
        var categories = await _categoryRepository.GetAllAsync(cancellationToken: cancellationToken);

        if (categories.Any())
            result = categories.ToDto();

        return Result<IEnumerable<CategoryListResponse>>.Success(result);
    }
}
