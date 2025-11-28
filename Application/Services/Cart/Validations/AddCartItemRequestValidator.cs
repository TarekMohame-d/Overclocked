using Application.Services.Cart.DTOs.Request;
using FluentValidation;

namespace Application.Services.Cart.Validations;

public class AddCartItemRequestValidator : AbstractValidator<AddCartItemRequestBody>
{
    public AddCartItemRequestValidator()
    {
        RuleFor(x => x.Quantity)
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .GreaterThan(0)
            .WithMessage("{PropertyName} must be greater than 0.");
    }
}
