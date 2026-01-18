using FluentValidation;

namespace Overclocked.Application.Features.UserUseCases.AddAddress;

public class AddAddressRequestValidator : AbstractValidator<AddAddressRequest>
{
    public AddAddressRequestValidator()
    {
        RuleFor(x => x.Building)
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .MaximumLength(30)
            .WithMessage("{PropertyName} must be at most 300 characters long.");

        RuleFor(x => x.Apartment)
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .GreaterThan(0)
            .WithMessage("{PropertyName} must be greater than 0.");

        RuleFor(x => x.Street)
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .MaximumLength(100)
            .WithMessage("{PropertyName} must be at most 100 characters long.");

        RuleFor(x => x.City)
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .MaximumLength(30)
            .WithMessage("{PropertyName} must be at most 30 characters long.");

        RuleFor(x => x.PostalCode)
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .MaximumLength(10)
            .WithMessage("{PropertyName} must be at most 10 characters long.");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .MaximumLength(300)
            .WithMessage("{PropertyName} must be at most 300 characters long.");
    }
}
