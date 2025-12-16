using System.Net;
using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Domain.Common.Results;
using CategoryEntity = Overclocked.Domain.CategoryAggregate.Category;

namespace Overclocked.Application.Category.Commands.CreateCategory;

public class CreateCategoryCommandHandler(
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<CreateCategoryCommand>
{
    public async Task<Result> Handle(CreateCategoryCommand command, CancellationToken cancellationToken)
    {
        var category = CategoryEntity.Create(command.Name, command.ImageUrl);

        await categoryRepository.AddAsync(category, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(HttpStatusCode.Created);
    }
}
