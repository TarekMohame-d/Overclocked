using System.Net;
using Overclocked.Application.Category.Commands.UpdateCategory;
using Overclocked.Domain.CategoryAggregate.ValueObjects;
using Overclocked.Domain.Common.Errors;
using Overclocked.Domain.Common.Results;

namespace Overclocked.Application.Category.Commands;

public sealed partial class CategoryCommands
{
    public async Task<Result> UpdateCategoryCommandHandler(
        UpdateCategoryCommand command,
        CancellationToken cancellationToken)
    {
        Domain.CategoryAggregate.Category? category = await categoryRepository
            .SingleOrDefaultAsync(x => x.Id == CategoryId.Create(command.Id), asNoTracking: false, cancellationToken);

        if(category is null)
        {
            return Result.Failure(CategoryErrors.CategoryNotFound(command.Id), HttpStatusCode.NotFound);
        }

        if(category.Name != command.Name)
        {
            var exist = await categoryRepository
                .AnyAsync(x => x.NormalizedName == command.Name.ToUpper(), cancellationToken);

            if(exist)
            {
                return Result.Failure(BrandErrors.BrandNameAlreadyExists, HttpStatusCode.Conflict);
            }
        }

        category.Update(command.Name, command.ImageUrl);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
