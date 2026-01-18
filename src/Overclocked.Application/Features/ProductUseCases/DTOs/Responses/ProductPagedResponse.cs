using Overclocked.Application.Features.BrandUseCases.DTOs.Responses;

namespace Overclocked.Application.Features.ProductUseCases.DTOs.Responses;

public record ProductPagedResponse
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Thumbnail { get; init; }
    public required decimal Price { get; init; }
    public required decimal Discount { get; init; }
    public decimal FinalPrice => Math.Round(Price * (1 - Discount), 2, MidpointRounding.ToEven);
    public required double Rating { get; init; }
    public required int ReviewCount { get; init; }
    public required BrandResponse Brand { get; init; }
}
