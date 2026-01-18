using FluentValidation;

namespace Overclocked.Application.Features.CartUseCases.AddCartItem;

public class AddCartItemRequestValidator : AbstractValidator<AddCartItemRequest>
{
    public AddCartItemRequestValidator()
    {
        RuleFor(x => x.Quantity)
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .GreaterThan(0)
            .WithMessage("{PropertyName} must be greater than 0.");

        RuleFor(x => x.ProductId).NotEmpty().WithMessage("{PropertyName} is required.");
    }
}
