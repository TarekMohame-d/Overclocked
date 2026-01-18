namespace Overclocked.Application.Features.CartUseCases.DTOs.Responses;

public record CartResponse
{
    public required IReadOnlyList<CartItemResponse> CartItems { get; init; }
    public required decimal Total { get; init; }
}
