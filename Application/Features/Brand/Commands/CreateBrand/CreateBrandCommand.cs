using Application.Abstraction.Messaging;
using Application.Common.Results;

namespace Application.Features.Brand.Commands.CreateBrand;

public record CreateBrandCommand : ICommand<Result>
{
    public required string Name { get; init; }
    public required string ImageUrl { get; init; }
}
