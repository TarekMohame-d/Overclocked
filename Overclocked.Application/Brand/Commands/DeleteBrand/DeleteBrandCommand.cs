using Overclocked.Domain.BrandAggregate.ValueObjects;

namespace Overclocked.Application.Brand.Commands.DeleteBrand;

public record DeleteBrandCommand(BrandId Id);
