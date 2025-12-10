namespace Overclocked.Application.Brand.Commands.DeleteBrand;

public record DeleteBrandCommand
{
    public required Guid Id { get; init; }
}
