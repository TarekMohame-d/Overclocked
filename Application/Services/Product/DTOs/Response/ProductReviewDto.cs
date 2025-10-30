namespace Application.Services.Product.DTOs.Response;

public record ProductReviewResponse
{
    public required Guid Id { get; init; }
    public required string UserName { get; init; }
    public required int Rating { get; init; }
    public required string Comment { get; init; }
    public required DateTime CreatedAt { get; init; }
    public ProductReviewReplyResponse? Reply { get; init; }
}

public record ProductReviewReplyResponse
{
    public required Guid Id { get; init; }
    public required string Reply { get; init; }
    public required DateTime CreatedAt { get; init; }
}
