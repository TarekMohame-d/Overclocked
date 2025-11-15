namespace Api.Routing;

public abstract class AuthRoutes : BaseRoute
{
    private const string Prefix = $"{Base}/auth";

    public const string Register = $"{Prefix}/register";
    public const string Login = $"{Prefix}/login";
    public const string ConfirmEmail = $"{Prefix}/confirm-email";
    public const string ForgotPassword = $"{Prefix}/forgot-password";
    public const string ResetPassword = $"{Prefix}/reset-password";
    public const string ResendConfirmationCode = $"{Prefix}/resend-confirmation-code";
    public const string RefreshToken = $"{Prefix}/refresh-token";
}
