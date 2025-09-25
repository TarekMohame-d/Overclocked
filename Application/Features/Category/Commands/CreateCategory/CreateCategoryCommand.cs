using Application.Abstraction.Messaging;
using Application.Common.Results;

namespace Application.Features.Category.Commands.CreateCategory;

public class CreateCategoryCommand : ICommand<Result>
{
    public string Name { get; init; } = default!;
    public string ImageUrl { get; init; } = default!;
}
