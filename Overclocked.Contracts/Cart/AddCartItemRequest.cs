namespace Overclocked.Contracts.Cart;

public record AddCartItemRequest
{
    public required Guid ProductId { get; init; }
    public required int Quantity { get; init; }
}
