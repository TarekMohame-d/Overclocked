using Application.Common.Results;
using Application.Services.Category.DTOs.Request;
using Application.Services.Category.DTOs.Response;
using Application.Services.Category.Mapping;

namespace Application.Services.Category;

public sealed partial class CategoryService
{
    public async Task<Result<IEnumerable<CategoryListResponse>>> GetAllCategoriesAsync(
        GetAllCategoriesRequest request,
        CancellationToken cancellationToken
    )
    {
        IEnumerable<CategoryListResponse> result = [];
        IEnumerable<Domain.Entities.Category> categories = await categoryRepository.GetAllAsync(
            cancellationToken: cancellationToken
        );

        if(categories.Any())
            result = categories.ToDto();

        return Result<IEnumerable<CategoryListResponse>>.Success(result);
    }
}
