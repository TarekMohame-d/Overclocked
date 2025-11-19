using System.Net;
using Application.Common.Results;
using Application.Common.Results.PredefinedErrors;
using Application.Services.Category.DTOs.Request;
using Application.Services.Category.Events;

namespace Application.Services.Category;

public sealed partial class CategoryService
{
    public async Task<Result> DeleteCategoryAsync(DeleteCategoryRequest request, CancellationToken cancellationToken)
    {
        Domain.Entities.Category? category = await categoryRepository.GetByIdAsync([request.Id], cancellationToken);

        if(category is null)
        {
            return Result.Failure(Errors.CategoryNotFound, HttpStatusCode.NotFound);
        }

        categoryRepository.Delete(category);

        CategoryDeletedEvent categoryDeletedEvent = new(category.Image);
        await eventDispatcher.DispatchAsync(categoryDeletedEvent, cancellationToken);

        await unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success();
    }
}
