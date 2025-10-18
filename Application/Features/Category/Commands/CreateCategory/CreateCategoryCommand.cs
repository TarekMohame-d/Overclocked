using Application.Abstraction.Messaging;
using Application.Common.Results;

namespace Application.Features.Category.Commands.CreateCategory;

public class CreateCategoryCommand : ICommand<Result>
{
    public required string Name { get; init; }
    public required string ImageUrl { get; init; }
}
