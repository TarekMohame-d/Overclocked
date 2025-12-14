namespace Overclocked.Contracts.Cart;

public record CartResponse
{
    public required IEnumerable<CartItemResponse> CartItems { get; init; }
    public required decimal Total { get; init; }

    public record CartItemResponse
    {
        public required Guid CartItemId { get; init; }
        public required Guid ProductId { get; init; }
        public required string ProductName { get; init; }
        public required string ProductDescription { get; init; }
        public required string ProductThumbnail { get; init; }
        public required int Quantity { get; init; }
        public required decimal UnitPrice { get; init; }
        public required decimal Discount { get; init; }
        public required decimal LineTotal { get; init; }
    }
}
