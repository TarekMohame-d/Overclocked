using Overclocked.SharedKernel;

namespace Overclocked.Domain.BrandAggregate;

public static class BrandErrors
{
    public static Error BrandNotFound(Guid id) => Error.NotFound("Brand.NotFound", $"The Brand with Id: '{id}' was not found.");

    public static Error BrandNameAlreadyExists => Error.Conflict("Brand.NameAlreadyExists", "Name already exists.");

    public static Error BrandNameIsRequired => Error.Validation("Brand.NameIsRequired", "Name is required.");

    public static Error BrandNameIsTooLong => Error.Validation("Brand.NameIsTooLong", "Name must be less than 50 characters.");
}
