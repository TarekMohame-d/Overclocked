namespace Application.Services.Brand.DTOs.Request;

public record DeleteBrandRequest
{
    public required Guid Id { get; init; }
}
