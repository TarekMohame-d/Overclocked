using System.Net;
using Application.Common.Results;
using Application.Services.Category.DTOs.Request;
using Application.Services.Category.Events;
using Application.Services.Category.Mapping;

namespace Application.Services.Category;

public sealed partial class CategoryService
{
    public async Task<Result> UpdateCategoryAsync(UpdateCategoryRequest request, CancellationToken cancellationToken)
    {
        Domain.Entities.Category? category = await categoryRepository.GetByIdAsync([request.Id], cancellationToken);

        if (category is null)
            return Result.Failure(Errors.CategoryNotFound, HttpStatusCode.NotFound);

        if (category.Name != request.Name)
        {
            var exist = await categoryRepository
                .AnyAsync(x => x.NormalizedName == request.Name.ToUpper(), cancellationToken);

            if (exist)
                return Result.Failure(Errors.CategoryNameAlreadyExists, HttpStatusCode.Conflict);
        }

        // Delete old image
        if (category.Image != request.ImageUrl)
        {
            CategoryUpdatedEvent categoryUpdatedEvent = new(category.Image);
            await eventDispatcher.DispatchAsync(categoryUpdatedEvent, cancellationToken);
        }

        category.UpdateFrom(request);

        categoryRepository.Update(category);

        await unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success();
    }
}
