using Application.Abstraction.Messaging;
using Application.Abstraction.Repositories;
using Application.Common.Results;
using Application.Features.Category.Mapping;
namespace Application.Features.Category.Queries.GetAllCategories;

public class GetAllCategoriesQueryHandler : IQueryHandler<GetAllCategoriesQuery, Result<IEnumerable<CategoryListDto>>>
{
    private readonly ICategoryRepository _categoryRepository;

    public GetAllCategoriesQueryHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<Result<IEnumerable<CategoryListDto>>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<CategoryListDto> result = [];

        var categories = await _categoryRepository.GetAllAsync(cancellationToken: cancellationToken);

        if (categories.Any())
            result = categories.ToDto();

        return Result<IEnumerable<CategoryListDto>>.Success(result);
    }
}
