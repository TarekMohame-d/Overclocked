using Overclocked.Domain.Common.Enums;
using Overclocked.Domain.Common.Results;

namespace Overclocked.Domain.Common.Errors;

public static class CartErrors
{
    public static Error CartItemNotFound(Guid id)
    {
        return new("Cart.ItemNotFound", $"The Cart Item with ID: '{id}' was not found.", ErrorType.NotFound);
    }
}
