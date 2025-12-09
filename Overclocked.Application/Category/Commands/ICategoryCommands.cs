using Overclocked.Application.Category.Commands.CreateCategory;
using Overclocked.Application.Category.Commands.DeleteCategory;
using Overclocked.Application.Category.Commands.UpdateCategory;
using Overclocked.Domain.Common.Results;

namespace Overclocked.Application.Category.Commands;

public interface ICategoryCommands
{
    Task<Result> CreateCategoryCommandHandler(CreateCategoryCommand command, CancellationToken cancellationToken);
    Task<Result> UpdateCategoryCommandHandler(UpdateCategoryCommand command, CancellationToken cancellationToken);
    Task<Result> DeleteCategoryCommandHandler(DeleteCategoryCommand command, CancellationToken cancellationToken);
}
