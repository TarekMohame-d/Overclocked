using System.Net;
using Overclocked.Application.Category.Commands.DeleteCategory;
using Overclocked.Domain.Common.Errors;
using Overclocked.Domain.Common.Results;

namespace Overclocked.Application.Category.Commands;

public sealed partial class CategoryCommands
{
    public async Task<Result> DeleteCategoryCommandHandler(
        DeleteCategoryCommand command,
        CancellationToken cancellationToken)
    {
        Domain.CategoryAggregate.Category? category = await categoryRepository
            .GetByIdAsync(command.Id, cancellationToken);

        if(category is null)
        {
            return Result.Failure(CategoryErrors.CategoryNotFound(command.Id), HttpStatusCode.NotFound);
        }

        category.Delete();
        categoryRepository.Delete(category);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
