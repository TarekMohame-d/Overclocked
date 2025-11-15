using Application.Services.Authentication.DTOs.Request;
using FluentValidation;

namespace Application.Services.Authentication.Validations;

public class ResendEmailConfirmationCodeRequestValidator : AbstractValidator<ResendEmailConfirmationCodeRequest>
{
    public ResendEmailConfirmationCodeRequestValidator()
    {
        RuleFor(x => x.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .EmailAddress()
            .WithMessage("{PropertyName} is not valid email address.")
            .MaximumLength(100)
            .WithMessage("{PropertyName} must be at most 100 characters long.");
    }
}
