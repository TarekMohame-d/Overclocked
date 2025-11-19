using Application.Services.Authentication.DTOs.Request;
using FluentValidation;

namespace Application.Services.Authentication.Validations;

public class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequest>
{
    public RefreshTokenRequestValidator()
    {
        RuleFor(x => x.AccessToken).Must(BeValidJwt).WithMessage("{PropertyName} is not a valid JWT.");

        RuleFor(x => x.RefreshToken).NotEmpty().WithMessage("{PropertyName} is required.");
    }

    private bool BeValidJwt(string token)
    {
        if(string.IsNullOrWhiteSpace(token))
            return false;

        var parts = token.Split('.');
        return parts.Length == 3 && parts[0].Length > 0 && parts[1].Length > 0 && parts[2].Length > 0;
    }
}
