using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MimeKit.Text;
using Overclocked.Application.Abstraction.Services;

namespace Overclocked.Infrastructure.Services;

public class EmailService(IConfiguration configuration) : IEmailService
{
    public async Task SendConfirmationCode(string to, string code)
    {
        var from = configuration["EmailSettings:From"];
        var appPassword = configuration["EmailSettings:AppPassword"];

        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(from));
        email.To.Add(MailboxAddress.Parse(to));
        email.Subject = "Overclocked Confirmation code";

        var body =
            $@"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Confirmation Code</title>
</head>
<body style=""font-family: 'Segoe UI', Arial, sans-serif; background-color: #f5f7fa; padding: 40px 0; margin: 0;"">
    <table align=""center"" width=""100%"" style=""max-width: 480px; background: #ffffff; border-radius: 12px; box-shadow: 0 4px 12px rgba(0,0,0,0.1);"">
        <tr>
            <td style=""padding: 32px 24px; text-align: center;"">
                <h1 style=""color: #2c3e50; margin-bottom: 24px;"">Overclocked</h1>
                <p style=""font-size: 16px; color: #555; margin-bottom: 32px;"">
                    Your confirmation code is:
                </p>
                <div style=""display: inline-block; background: #1e88e5; color: #fff; font-size: 24px;
                            letter-spacing: 4px; padding: 14px 28px; border-radius: 8px; font-weight: bold;"">
                    {code}
                </div>
                <p style=""color: #777; font-size: 14px; margin-top: 36px;"">
                    This code will expire soon. Please use it within the next 10 minutes.
                </p>
                <hr style=""border:none; border-top:1px solid #eee; margin: 36px 0;"" />
                <p style=""font-size: 12px; color: #999;"">
                    If you didn’t request this code, you can safely ignore this email.
                </p>
            </td>
        </tr>
    </table>
</body>
</html>";

        email.Body = new TextPart(TextFormat.Html) { Text = body };

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(from, appPassword);
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }
}
