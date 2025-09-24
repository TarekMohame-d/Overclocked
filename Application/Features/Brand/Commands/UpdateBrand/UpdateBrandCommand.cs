using Application.Abstraction.Messaging;
using Application.Common.Results;

namespace Application.Features.Brand.Commands.UpdateBrand;

public record UpdateBrandWithIdCommand : UpdateBrandCommand, ICommand<Result>
{
    public Guid Id { get; init; }
}

public record UpdateBrandCommand
{
    public required string Name { get; init; }
    public required string ImageUrl { get; init; }
}
