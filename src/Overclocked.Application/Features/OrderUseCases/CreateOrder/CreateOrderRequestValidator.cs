using FluentValidation;

namespace Overclocked.Application.Features.OrderUseCases.CreateOrder;

public class CreateOrderRequestValidator : AbstractValidator<CreateOrderRequest>
{
    public CreateOrderRequestValidator()
    {
        RuleFor(x => x.ShippingAddress.Building)
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .MaximumLength(30)
            .WithMessage("{PropertyName} must be at most 300 characters long.");

        RuleFor(x => x.ShippingAddress.Apartment)
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .GreaterThan(0)
            .WithMessage("{PropertyName} must be greater than 0.");

        RuleFor(x => x.ShippingAddress.Street)
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .MaximumLength(100)
            .WithMessage("{PropertyName} must be at most 100 characters long.");

        RuleFor(x => x.ShippingAddress.City)
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .MaximumLength(30)
            .WithMessage("{PropertyName} must be at most 30 characters long.");

        RuleFor(x => x.ShippingAddress.PostalCode)
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .MaximumLength(10)
            .WithMessage("{PropertyName} must be at most 10 characters long.");

        RuleFor(x => x.ShippingAddress.Description)
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .MaximumLength(300)
            .WithMessage("{PropertyName} must be at most 300 characters long.");
    }
}
