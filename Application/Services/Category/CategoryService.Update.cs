using System.Net;
using Application.Common.Results;
using Application.Common.Results.PredefinedErrors;
using Application.Services.Category.DTOs.Request;
using Application.Services.Category.Events;
using Application.Services.Category.Mapping;

namespace Application.Services.Category;

public sealed partial class CategoryService
{
    public async Task<Result> UpdateCategoryAsync(UpdateCategoryRequest request, CancellationToken cancellationToken)
    {
        Domain.Entities.Category? category = await categoryRepository
            .SingleOrDefaultAsync(x => x.Id == request.Id, asNoTracking: false, cancellationToken);

        if(category is null)
            return Result.Failure(Errors.CategoryNotFound, HttpStatusCode.NotFound);

        var oldImageUrl = category.Image;

        if(category.Name != request.Name)
        {
            var exist = await categoryRepository
                .AnyAsync(x => x.NormalizedName == request.Name.ToUpper(), cancellationToken);

            if(exist)
                return Result.Failure(Errors.CategoryNameAlreadyExists, HttpStatusCode.Conflict);
        }

        category.UpdateFrom(request);

        await unitOfWork.CompleteAsync(cancellationToken);

        // Delete old image if new image is different
        if(oldImageUrl != request.ImageUrl)
        {
            CategoryUpdatedEvent categoryUpdatedEvent = new(category.Image);
            await eventDispatcher.DispatchAsync(categoryUpdatedEvent, cancellationToken);
        }

        return Result.Success();
    }
}
