using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Domain.CategoryAggregate;
using Overclocked.Domain.Common.Shared.ValueObjects.Image;
using Overclocked.SharedKernel;

namespace Overclocked.Application.Features.CategoryUseCases.CreateCategory;

public class CreateCategoryRequestHandler(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<CreateCategoryRequest, Guid>
{
    public async Task<Result<Guid>> Handle(CreateCategoryRequest request, CancellationToken ct)
    {
        if (await categoryRepository.NameExistsAsync(request.Name, ct))
            return Result.Failure<Guid>(CategoryErrors.CategoryNameAlreadyExists);

        Result<Image> imageResult = Image.Create(request.ImageUrl);

        if (imageResult.IsFailure)
            return Result.Failure<Guid>(imageResult.Error);

        Result<Category> brandResult = Category.Create(request.Name, imageResult.Value);

        if (brandResult.IsFailure)
            return Result.Failure<Guid>(brandResult.Error);

        Category brand = brandResult.Value;
        categoryRepository.Add(brand);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(brand.Id.Value);
    }
}
