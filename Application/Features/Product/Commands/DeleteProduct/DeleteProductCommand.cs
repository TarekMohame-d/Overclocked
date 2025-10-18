using Application.Abstraction.Messaging;
using Application.Common.Results;

namespace Application.Features.Product.Commands.DeleteProduct;

public record DeleteProductCommand : ICommand<Result>
{
    public Guid Id { get; init; }
}
