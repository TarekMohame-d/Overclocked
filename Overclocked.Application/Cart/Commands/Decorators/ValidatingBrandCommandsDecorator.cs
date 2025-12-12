using FluentValidation;
using FluentValidation.Results;
using Overclocked.Application.Cart.Commands.AddCartItem;
using Overclocked.Contracts.Cart;
using Overclocked.Domain.Common.Results;

namespace Overclocked.Application.Cart.Commands.Decorators;

public class ValidatingCartCommandsDecorator(ICartCommands inner,
        IValidator<AddCartItemCommand> addCartItemValidator) : ICartCommands
{
    public async Task<Result<CartItemResponse>> AddCartItemCommandHandler(
        AddCartItemCommand command,
        CancellationToken cancellationToken)
    {
        ValidationResult validationResult = await addCartItemValidator.ValidateAsync(command, cancellationToken);

        if(!validationResult.IsValid)
        {
            var errorDictionary = validationResult
                .Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

            return Result<CartItemResponse>.ValidationError<AddCartItemCommand>(errorDictionary);
        }

        Result<CartItemResponse> result = await inner.AddCartItemCommandHandler(command, cancellationToken);

        return result;
    }
}
