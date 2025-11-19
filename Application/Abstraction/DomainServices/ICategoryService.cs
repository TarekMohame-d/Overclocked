using Application.Common.Results;
using Application.Services.Category.DTOs.Request;
using Application.Services.Category.DTOs.Response;

namespace Application.Abstraction.DomainServices;

public interface ICategoryService
{
    Task<Result<CategoryResponse>> GetCategoryByIdAsync(
        GetCategoryByIdRequest request,
        CancellationToken cancellationToken
    );

    Task<Result<IEnumerable<CategoryListResponse>>> GetAllCategoriesAsync(
        GetAllCategoriesRequest request,
        CancellationToken cancellationToken
    );

    Task<Result> CreateCategoryAsync(CreateCategoryRequest request, CancellationToken cancellationToken);
    Task<Result> UpdateCategoryAsync(UpdateCategoryRequest request, CancellationToken cancellationToken);
    Task<Result> DeleteCategoryAsync(DeleteCategoryRequest request, CancellationToken cancellationToken);
}
