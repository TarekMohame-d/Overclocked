using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Domain.CategoryAggregate.ValueObjects;
using Overclocked.Domain.Common.Errors;
using Overclocked.Domain.Common.Results;

namespace Overclocked.Application.Category.Commands.UpdateCategory;

public class UpdateCategoryCommandHandler(
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<UpdateCategoryCommand>
{
    public async Task<Result> Handle(UpdateCategoryCommand command, CancellationToken cancellationToken)
    {
        Domain.CategoryAggregate.Category? category = await categoryRepository
            .FindAsync(CategoryId.Create(command.Id), cancellationToken);

        if(category is null)
        {
            return Result.Failure(CategoryErrors.CategoryNotFound(command.Id));
        }

        if(category.Name != command.Name)
        {
            var exist = await categoryRepository
                .AnyAsync(x => x.NormalizedName == command.Name.ToUpper(), cancellationToken);

            if(exist)
            {
                return Result.Failure(CategoryErrors.CategoryNameAlreadyExists);
            }
        }

        category.Update(command.Name, command.ImageUrl);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
