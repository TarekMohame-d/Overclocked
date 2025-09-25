using Application.Abstraction.Messaging;
using Application.Common.Results;

namespace Application.Features.Category.Commands.DeleteCategory;

public record DeleteCategoryCommand : ICommand<Result>
{
    public Guid Id { get; init; }
}
