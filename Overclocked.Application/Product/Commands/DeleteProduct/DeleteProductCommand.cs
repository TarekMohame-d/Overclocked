namespace Overclocked.Application.Product.Commands.DeleteProduct;

public record DeleteProductCommand
{
    public required Guid Id { get; init; }
}
