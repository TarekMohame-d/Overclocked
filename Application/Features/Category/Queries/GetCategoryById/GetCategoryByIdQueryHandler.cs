using Application.Abstraction.Messaging;
using Application.Common.Results;
using Application.Features.Category.Mapping;
using Domain.Repositories;
using System.Net;

namespace Application.Features.Category.Queries.GetCategoryById;

public class GetCategoryByIdQueryHandler : IQueryHandler<GetCategoryByIdQuery, Result<CategoryDto>>
{
    private readonly ICategoryRepository _categoryRepository;

    public GetCategoryByIdQueryHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<Result<CategoryDto>> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync([request.Id], cancellationToken);

        if (category is null)
            return Result<CategoryDto>.Failure(Errors.CategoryNotFound, HttpStatusCode.NotFound);

        return category.ToDto();
    }
}
