using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Domain.CategoryAggregate;
using Overclocked.Domain.CategoryAggregate.ValueObjects;
using Overclocked.Domain.Common.Shared.ValueObjects.Image;
using Overclocked.SharedKernel;

namespace Overclocked.Application.Features.CategoryUseCases.UpdateCategory;

public class UpdateCategoryRequestHandler(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateCategoryRequest>
{
    public async Task<Result> Handle(UpdateCategoryRequest request, CancellationToken ct)
    {
        Result<Image> imageResult = Image.Create(request.ImageUrl);

        if (imageResult.IsFailure)
            return Result.Failure<Guid>(imageResult.Error);

        Category? category = await categoryRepository.GetByIdAsync(CategoryId.Create(request.Id), ct);

        if (category is null)
            return Result.Failure(CategoryErrors.CategoryNotFound(request.Id));

        if (category.Name != request.Name)
        {
            var exist = await categoryRepository.NameExistsAsync(request.Name, ct);

            if (exist)
                return Result.Failure(CategoryErrors.CategoryNameAlreadyExists);
        }

        Result result = category.Update(request.Name, imageResult.Value);

        if (result.IsFailure)
            return Result.Failure(result.Error);

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
