namespace Application.Services.Product.DTOs.Request;

public record DeleteProductRequest
{
    public required Guid Id { get; init; }
}
