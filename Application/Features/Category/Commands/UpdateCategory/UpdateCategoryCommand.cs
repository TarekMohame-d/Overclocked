using Application.Abstraction.Messaging;
using Application.Common.Results;

namespace Application.Features.Category.Commands.UpdateCategory;

public record UpdateCategoryWithIdCommand : UpdateCategoryCommand, ICommand<Result>
{
    public Guid Id { get; init; }
}

public record UpdateCategoryCommand
{
    public string Name { get; init; } = default!;
    public string ImageUrl { get; init; } = default!;
}

