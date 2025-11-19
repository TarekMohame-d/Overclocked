using System.Net;
using Application.Common.Results;
using Application.Services.Category.DTOs.Request;
using Application.Services.Category.Mapping;

namespace Application.Services.Category;

public sealed partial class CategoryService
{
    public async Task<Result> CreateCategoryAsync(CreateCategoryRequest request, CancellationToken cancellationToken)
    {
        Domain.Entities.Category brand = request.ToEntity();

        await categoryRepository.AddAsync(brand, cancellationToken);

        await unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success(HttpStatusCode.Created);
    }
}
