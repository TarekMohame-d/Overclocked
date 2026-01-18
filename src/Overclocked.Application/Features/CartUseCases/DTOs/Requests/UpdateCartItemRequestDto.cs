namespace Overclocked.Application.Features.CartUseCases.DTOs.Requests;

public record UpdateCartItemRequestDto
{
    public required int Quantity { get; init; }
}
