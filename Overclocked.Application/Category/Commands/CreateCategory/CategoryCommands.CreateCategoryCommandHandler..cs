using System.Net;
using Overclocked.Application.Category.Commands.CreateCategory;
using Overclocked.Domain.CategoryAggregate.ValueObjects;
using Overclocked.Domain.Common.Results;
using CategoryEntity = Overclocked.Domain.CategoryAggregate.Category;

namespace Overclocked.Application.Category.Commands;

public sealed partial class CategoryCommands
{
    public async Task<Result> CreateCategoryCommandHandler(
        CreateCategoryCommand command,
        CancellationToken cancellationToken)
    {
        var category = CategoryEntity.Create(CategoryId.Create(), command.Name, command.ImageUrl);

        await categoryRepository.AddAsync(category, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(HttpStatusCode.Created);
    }
}
