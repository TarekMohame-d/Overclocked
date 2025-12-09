using FluentValidation;

namespace Overclocked.Application.Authentication.Commands.ResendEmailConfirmationCode;

public class ResendEmailConfirmationCodeCommandValidator : AbstractValidator<ResendEmailConfirmationCodeCommand>
{
    public ResendEmailConfirmationCodeCommandValidator()
    {
        RuleFor(x => x.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .Matches(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$")
            .WithMessage("{PropertyName} is not valid email address.")
            .MaximumLength(100)
            .WithMessage("{PropertyName} must be at most 100 characters long.");
    }
}
