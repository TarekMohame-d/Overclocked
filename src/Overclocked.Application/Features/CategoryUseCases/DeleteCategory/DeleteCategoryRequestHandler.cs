using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Domain.CategoryAggregate;
using Overclocked.Domain.CategoryAggregate.ValueObjects;
using Overclocked.SharedKernel;

namespace Overclocked.Application.Features.CategoryUseCases.DeleteCategory;

public class DeleteCategoryRequestHandler(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteCategoryRequest>
{
    public async Task<Result> Handle(DeleteCategoryRequest request, CancellationToken ct)
    {
        Category? category = await categoryRepository.GetByIdAsync(CategoryId.Create(request.Id), ct);

        if (category is null)
            return Result.Failure(CategoryErrors.CategoryNotFound(request.Id));

        category.DeleteCategoryImage();
        categoryRepository.Remove(category);

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
