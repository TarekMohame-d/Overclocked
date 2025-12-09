using Overclocked.Domain.BrandAggregate.ValueObjects;

namespace Overclocked.Application.Brand.Commands.UpdateBrand;

public record UpdateBrandCommand(BrandId Id, string Name, string ImageUrl);
