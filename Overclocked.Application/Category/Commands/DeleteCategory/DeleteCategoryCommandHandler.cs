using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Domain.CategoryAggregate.ValueObjects;
using Overclocked.Domain.Common.Errors;
using Overclocked.Domain.Common.Results;

namespace Overclocked.Application.Category.Commands.DeleteCategory;

public class DeleteCategoryCommandHandler(
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<DeleteCategoryCommand>
{
    public async Task<Result> Handle(DeleteCategoryCommand command, CancellationToken cancellationToken)
    {
        Domain.CategoryAggregate.Category? category = await categoryRepository
            .FindAsync(CategoryId.Create(command.Id), cancellationToken);

        if(category is null)
        {
            return Result.Failure(CategoryErrors.CategoryNotFound(command.Id));
        }

        category.Delete();
        categoryRepository.Delete(category);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
