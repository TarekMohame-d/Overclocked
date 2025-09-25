using Application.Abstraction.Messaging;
using Application.Common.Results;

namespace Application.Features.Brand.Commands.CreateBrand;

public record CreateBrandCommand : ICommand<Result>
{
    public string Name { get; init; } = default!;
    public string ImageUrl { get; init; } = default!;
}
