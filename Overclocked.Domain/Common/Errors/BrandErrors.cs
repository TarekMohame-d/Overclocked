using Overclocked.Domain.Common.Enums;
using Overclocked.Domain.Common.Results;

namespace Overclocked.Domain.Common.Errors;

public static class BrandErrors
{
    public static Error BrandNotFound(Guid id)
    {
        return new(nameof(BrandNotFound), ErrorType.NotFound, $"The brand with ID '{id}' was not found.");
    }

    public static readonly Error BrandNameAlreadyExists = new(
        nameof(BrandNameAlreadyExists),
        ErrorType.Conflict,
        "Brand name already exists.");
}
