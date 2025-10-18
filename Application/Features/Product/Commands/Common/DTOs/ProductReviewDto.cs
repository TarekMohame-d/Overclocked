namespace Application.Features.Product.Commands.Common.DTOs;

public record ProductReviewDto
{
    public Guid Id { get; init; }
    public string UserName { get; init; } = string.Empty;
    public int Rating { get; init; }
    public string Comment { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public ProductReviewReplyDto? Reply { get; init; }
}

public record ProductReviewReplyDto
{
    public Guid Id { get; init; }
    public string Reply { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
}
