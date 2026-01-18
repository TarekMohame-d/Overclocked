namespace Overclocked.Application.Features.WishlistUseCases.DTOs.Responses;

public record WishlistItemResponse
{
    public required Guid ProductId { get; init; }
    public required string ProductName { get; init; }
    public required string ProductDescription { get; init; }
    public required string ProductThumbnail { get; init; }
}
