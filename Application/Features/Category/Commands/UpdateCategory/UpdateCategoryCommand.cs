using Application.Abstraction.Messaging;
using Application.Common.Results;

namespace Application.Features.Category.Commands.UpdateCategory;

public record UpdateCategoryWithIdCommand : UpdateCategoryCommand, ICommand<Result>
{
    public Guid Id { get; init; }
}

public record UpdateCategoryCommand
{
    public required string Name { get; init; }
    public required string ImageUrl { get; init; }
}

