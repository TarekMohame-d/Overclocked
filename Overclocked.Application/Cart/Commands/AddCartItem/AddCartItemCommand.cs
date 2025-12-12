namespace Overclocked.Application.Cart.Commands.AddCartItem;

public record AddCartItemCommand
{
    public required Guid UserId { get; init; }
    public required Guid ProductId { get; init; }
    public required int Quantity { get; init; }
}
