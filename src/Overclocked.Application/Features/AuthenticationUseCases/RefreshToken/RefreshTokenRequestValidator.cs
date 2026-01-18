using FluentValidation;

namespace Overclocked.Application.Features.AuthenticationUseCases.RefreshToken;

public class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequest>
{
    public RefreshTokenRequestValidator()
    {
        RuleFor(x => x.AccessToken)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .Must(BeValidJwt)
            .WithMessage("{PropertyName} is not a valid JWT.");

        RuleFor(x => x.RefreshToken).NotEmpty().WithMessage("{PropertyName} is required.");
    }

    private static bool BeValidJwt(string token)
    {
        var parts = token.Split('.');
        return parts.Length == 3 && parts[0].Length > 0 && parts[1].Length > 0 && parts[2].Length > 0;
    }
}
