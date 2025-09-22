using Application.Abstraction.Messaging;
using Application.Common.Results;

namespace Application.Features.Brand.Commands.DeleteBrand;

public record DeleteBrandCommand : ICommand<Result>
{
    public Guid Id { get; init; }
}
