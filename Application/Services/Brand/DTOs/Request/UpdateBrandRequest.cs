namespace Application.Services.Brand.DTOs.Request;

public record UpdateBrandRequestBody
{
    public required string Name { get; init; }
    public required string ImageUrl { get; init; }
}

public record UpdateBrandRequest : UpdateBrandRequestBody
{
    public required Guid Id { get; init; }
}
