using Overclocked.Domain.Common.Enums;
using Overclocked.Domain.Common.Results;

namespace Overclocked.Domain.Common.Errors;

public static class ProductErrors
{
    public static Error ProductNotFound(Guid id)
    {
        return new(nameof(ProductNotFound), ErrorType.NotFound, $"The product with ID '{id}' was not found.");
    }

    public static readonly Error ProductNameAlreadyExists = new(
        nameof(ProductNameAlreadyExists),
        ErrorType.Conflict,
        "Product name already exists.");
}
