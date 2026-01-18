using FluentValidation;

namespace Overclocked.Application.Features.AuthenticationUseCases.Register;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .Matches(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$")
            .WithMessage("{PropertyName} is not valid email address.")
            .MaximumLength(100)
            .WithMessage("{PropertyName} must be at most 100 characters long.");

        RuleFor(x => x.Password)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .MinimumLength(8)
            .WithMessage("{PropertyName} must be at least 8 characters long.")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*(),.?""':{}|<>]).{8,}$")
            .WithMessage(
                "{PropertyName} must contain at least one uppercase letter, one lowercase letter, one number, and one special character."
            );

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .MaximumLength(20)
            .WithMessage("{PropertyName} must be at most 20 characters long.");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .MaximumLength(20)
            .WithMessage("{PropertyName} must be at most 20 characters long.");

        RuleFor(x => x.PhoneNumber)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .Matches(@"^\+?\d{10,15}$")
            .WithMessage("{PropertyName} must be a valid phone number.")
            .MaximumLength(20)
            .WithMessage("{PropertyName} must be at most 20 characters long.");
    }
}
