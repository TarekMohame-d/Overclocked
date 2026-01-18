namespace Overclocked.Application.Features.CartUseCases.DTOs.Requests;

public record AddCartItemRequestDto
{
    public required Guid ProductId { get; init; }
    public required int Quantity { get; init; }
}
