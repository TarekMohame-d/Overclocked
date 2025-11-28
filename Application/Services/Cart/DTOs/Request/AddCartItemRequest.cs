namespace Application.Services.Cart.DTOs.Request;

public record AddCartItemRequestBody
{
    public required Guid ProductId { get; init; }
    public required int Quantity { get; init; }
}

public record AddCartItemRequest : AddCartItemRequestBody
{
    public required Guid UserId { get; init; }
}
