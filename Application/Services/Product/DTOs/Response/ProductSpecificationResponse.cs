namespace Application.Services.Product.DTOs.Response;

public record ProductSpecificationResponse
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Value { get; init; }
}
