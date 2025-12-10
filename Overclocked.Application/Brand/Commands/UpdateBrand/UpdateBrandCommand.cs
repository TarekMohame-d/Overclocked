namespace Overclocked.Application.Brand.Commands.UpdateBrand;

public record UpdateBrandCommand
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string ImageUrl { get; init; }
}
