using Application.Services.Authentication.DTOs.Request;
using FluentValidation;

namespace Application.Services.Authentication.Validations;

public class ForgetPasswordRequestValidator : AbstractValidator<ForgetPasswordRequest>
{
    public ForgetPasswordRequestValidator()
    {
        RuleFor(x => x.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .EmailAddress()
            .WithMessage("{PropertyName} is not valid email address.");
    }
}
