using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Features.CategoryUseCases.DTOs.Responses;
using Overclocked.Application.Features.CategoryUseCases.Mapping;
using Overclocked.Domain.CategoryAggregate;
using Overclocked.SharedKernel;

namespace Overclocked.Application.Features.CategoryUseCases.GetAllCategories;

public class GetAllCategoriesRequestHandler(ICategoryReadRepository categoryRepository)
    : IRequestHandler<GetAllCategoriesRequest, IEnumerable<CategoryListResponse>>
{
    public async Task<Result<IEnumerable<CategoryListResponse>>> Handle(GetAllCategoriesRequest request, CancellationToken ct)
    {
        IEnumerable<Category> result = await categoryRepository.GetAllAsync(ct);

        return Result.Success(result.ToDto());
    }
}
