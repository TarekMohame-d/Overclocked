namespace Overclocked.Application.Features.ProductUseCases.DTOs.Responses;

public record ProductSpecificationDto
{
    public required string Name { get; init; }
    public required string Value { get; init; }
}
