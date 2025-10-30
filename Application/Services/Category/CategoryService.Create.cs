using System.Net;
using Application.Common.Results;
using Application.Features.Category.Mapping;
using Application.Services.Category.DTOs.Request;

namespace Application.Services.Category;

public sealed partial class CategoryService
{
    public async Task<Result> CreateCategoryAsync(CreateCategoryRequest request, CancellationToken cancellationToken)
    {
        var brand = request.ToEntity();

        await _categoryRepository.AddAsync(brand, cancellationToken);

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success(HttpStatusCode.Created);
    }
}
