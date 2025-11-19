using Application.Services.Authentication.DTOs.Request;
using FluentValidation;

namespace Application.Services.Authentication.Validations;

public class ResetPasswordRequestValidator : AbstractValidator<ResetPasswordRequest>
{
    public ResetPasswordRequestValidator()
    {
        RuleFor(x => x.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .EmailAddress()
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

        RuleFor(x => x.Code)
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .Length(6)
            .WithMessage("{PropertyName} must be 6 characters long.");
    }
}
