namespace Overclocked.Application.Brand.Commands.CreateBrand;

public record CreateBrandCommand
{
    public required string Name { get; init; }
    public required string ImageUrl { get; init; }
}
