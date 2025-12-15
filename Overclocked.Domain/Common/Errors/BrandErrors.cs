using Overclocked.Domain.Common.Enums;
using Overclocked.Domain.Common.Results;

namespace Overclocked.Domain.Common.Errors;

public static class BrandErrors
{
    public static Error BrandNotFound(Guid id)
    {
        return new("Brand.NotFound", $"The Brand with Id: '{id}' was not found.", ErrorType.NotFound);
    }

    public static readonly Error BrandNameAlreadyExists = new(
        "Brand.NameAlreadyExists",
        "Brand name already exists.",
        ErrorType.Conflict);
}
