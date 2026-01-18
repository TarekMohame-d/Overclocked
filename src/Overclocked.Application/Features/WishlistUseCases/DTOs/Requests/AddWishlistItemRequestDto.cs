namespace Overclocked.Application.Features.WishlistUseCases.DTOs.Requests;

public record AddWishlistItemRequestDto
{
    public required Guid ProductId { get; init; }
}
