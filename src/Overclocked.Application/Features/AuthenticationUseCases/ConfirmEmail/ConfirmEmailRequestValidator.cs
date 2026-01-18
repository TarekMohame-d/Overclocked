using FluentValidation;

namespace Overclocked.Application.Features.AuthenticationUseCases.ConfirmEmail;

public class ConfirmEmailRequestValidator : AbstractValidator<ConfirmEmailRequest>
{
    public ConfirmEmailRequestValidator()
    {
        RuleFor(x => x.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .Matches(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$")
            .WithMessage("{PropertyName} is not valid email address.")
            .MaximumLength(100)
            .WithMessage("{PropertyName} must be at most 100 characters long.");

        RuleFor(x => x.Code)
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .Length(6)
            .WithMessage("{PropertyName} must be 6 characters long.");
    }
}
