using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Features.CategoryUseCases.DTOs.Responses;
using Overclocked.Application.Features.CategoryUseCases.Mapping;
using Overclocked.Domain.CategoryAggregate;
using Overclocked.Domain.CategoryAggregate.ValueObjects;
using Overclocked.SharedKernel;

namespace Overclocked.Application.Features.CategoryUseCases.GetCategoryById;

public class GetCategoryByIdRequestHandler(ICategoryReadRepository categoryRepository)
    : IRequestHandler<GetCategoryByIdRequest, CategoryResponse>
{
    public async Task<Result<CategoryResponse>> Handle(GetCategoryByIdRequest request, CancellationToken ct)
    {
        Category? category = await categoryRepository.GetByIdAsync(CategoryId.Create(request.Id), ct);

        return category is null
            ? Result.Failure<CategoryResponse>(CategoryErrors.CategoryNotFound(request.Id))
            : Result.Success(category.ToDto());
    }
}
