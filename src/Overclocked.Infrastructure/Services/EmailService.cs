using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MimeKit.Text;
using Overclocked.Application.Abstractions.Services;

namespace Overclocked.Infrastructure.Services;

public class EmailService(IConfiguration configuration) : IEmailService
{
    public async Task SendConfirmationCode(string to, string code)
    {
        var fromAddress = configuration["EmailSettings:From"];
        var appPassword = configuration["EmailSettings:AppPassword"];

        var email = new MimeMessage();

        email.From.Add(new MailboxAddress("Overclocked (No Reply)", fromAddress));
        email.ReplyTo.Add(MailboxAddress.Parse("no-reply@overclocked.com"));

        email.To.Add(MailboxAddress.Parse(to));
        email.Subject = "Overclocked Confirmation code";

        var body = $"""

<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Confirmation Code</title>
</head>
<body style="font-family: 'Segoe UI', Arial, sans-serif; background-color: #f5f7fa; padding: 40px 0; margin: 0;">
    <table align="center" width="100%" style="max-width: 480px; background: #ffffff; border-radius: 12px; box-shadow: 0 4px 12px rgba(0,0,0,0.1);">
        <tr>
            <td style="padding: 32px 24px; text-align: center;">
                <h1 style="color: #2c3e50; margin-bottom: 24px;">Overclocked</h1>
                <p style="font-size: 16px; color: #555; margin-bottom: 32px;">
                    Your confirmation code is:
                </p>
                <div style="display: inline-block; background: #1e88e5; color: #fff; font-size: 24px;
                            letter-spacing: 4px; padding: 14px 28px; border-radius: 8px; font-weight: bold;">
                    {code}
                </div>
                <p style="color: #777; font-size: 14px; margin-top: 36px;">
                    This code will expire soon. Please use it within the next 10 minutes.
                </p>
                <hr style="border:none; border-top:1px solid #eee; margin: 36px 0;" />
                <p style="font-size: 12px; color: #999;">
                    If you didn’t request this code, you can safely ignore this email.
                </p>
            </td>
        </tr>
    </table>
</body>
</html>
""";

        email.Body = new TextPart(TextFormat.Html) { Text = body };

        using var smtp = new SmtpClient();
        try
        {
            await smtp.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(fromAddress, appPassword);
            await smtp.SendAsync(email);
        }
        catch (Exception ex)
        {
            //TODO: Add logging
            throw new Exception("Failed to send email. Check internet connection or SMTP credentials.", ex);
        }
        finally
        {
            await smtp.DisconnectAsync(true);
        }
    }

    public async Task SendOrderCancellationEmail(string to, string orderId)
    {
        var fromAddress = configuration["EmailSettings:From"];
        var appPassword = configuration["EmailSettings:AppPassword"];

        var email = new MimeMessage();
        email.From.Add(new MailboxAddress("Overclocked (No Reply)", fromAddress));
        email.ReplyTo.Add(MailboxAddress.Parse("no-reply@overclocked.com"));

        email.To.Add(MailboxAddress.Parse(to));
        email.Subject = $"Order Cancelled - #{orderId}";

        var body = $"""
<!DOCTYPE html>
<html lang="en">
<body style="font-family: 'Segoe UI', Arial, sans-serif; background-color: #f5f7fa; padding: 40px 0; margin: 0;">
    <table align="center" width="100%" style="max-width: 480px; background: #ffffff; border-radius: 12px; box-shadow: 0 4px 12px rgba(0,0,0,0.1);">
        <tr>
            <td style="padding: 40px 30px; text-align: center;">

                <!-- Red Title -->
                <h1 style="color: #e53935; margin: 0 0 24px 0; font-size: 24px; font-weight: 600;">
                    Order Cancelled
                </h1>

                <!-- Main Message -->
                <p style="font-size: 16px; color: #444; margin-bottom: 24px; line-height: 1.6;">
                    We're writing to confirm that your order <strong>#{orderId}</strong> has been cancelled.
                </p>

                <!-- Help/Refund Text -->
                <p style="color: #666; font-size: 14px; line-height: 1.6; margin-bottom: 30px;">
                    If you didn't request this cancellation or have questions about a refund (if payment was previously processed), please contact our support team immediately.
                </p>

                <hr style="border:none; border-top:1px solid #eee; margin: 30px 0;" />

                <!-- Footer -->
                <p style="font-size: 12px; color: #999; margin: 0;">
                    Overclocked - High Performance Gear
                </p>
            </td>
        </tr>
    </table>
</body>
</html>
""";

        email.Body = new TextPart(TextFormat.Html) { Text = body };

        using var smtp = new SmtpClient();
        try
        {
            await smtp.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(fromAddress, appPassword);
            await smtp.SendAsync(email);
        }
        finally
        {
            await smtp.DisconnectAsync(true);
        }
    }

    public async Task SendPaymentFailedEmail(string to, string orderId, string orderTotal)
    {
        var fromAddress = configuration["EmailSettings:From"];
        var appPassword = configuration["EmailSettings:AppPassword"];

        var email = new MimeMessage();
        email.From.Add(new MailboxAddress("Overclocked (No Reply)", fromAddress));
        email.ReplyTo.Add(MailboxAddress.Parse("no-reply@overclocked.com"));

        email.To.Add(MailboxAddress.Parse(to));
        email.Subject = $"Action Required: Payment failed for Order #{orderId}";

        var body = $"""
<!DOCTYPE html>
<html lang="en">
<body style="font-family: 'Segoe UI', Arial, sans-serif; background-color: #f5f7fa; padding: 40px 0; margin: 0;">
    <table align="center" width="100%" style="max-width: 500px; background: #ffffff; border-radius: 8px; border: 1px solid #ddd; box-shadow: 0 2px 5px rgba(0,0,0,0.05);">
        <!-- Red Warning Header -->
        <tr>
            <td style="padding: 20px; background-color: #c40000; border-radius: 8px 8px 0 0; text-align: center;">
                <h2 style="color: #ffffff; margin: 0; font-size: 20px;">Payment Authorization Failed</h2>
            </td>
        </tr>

        <tr>
            <td style="padding: 30px 25px;">
                <p style="font-size: 16px; color: #333; margin-top: 0;">Hello,</p>

                <p style="font-size: 15px; color: #333; line-height: 1.5;">
                    We are writing to let you know that the payment for your order <strong>#{orderId}</strong> was declined by your bank.
                </p>

                <div style="background-color: #fff3cd; border: 1px solid #ffeeba; color: #856404; padding: 15px; border-radius: 4px; margin: 20px 0; font-size: 14px;">
                    <strong>Why did this happen?</strong><br>
                    Common reasons include a low card balance, incorrect expiration date/CVV, or bank restrictions on online transactions.
                </div>

                <p style="font-size: 16px; color: #c40000; font-weight: bold; text-align: center;">
                    To keep your order from being cancelled, please provide a valid payment method within the next 30 minutes.
                </p>

                <p style="font-size: 14px; color: #555;">
                    Total to pay: <strong>{orderTotal}</strong>
                </p>

                <hr style="border:none; border-top:1px solid #eee; margin: 25px 0;" />

                <p style="font-size: 12px; color: #999; text-align: center;">
                    Overclocked - High Performance Gear
                </p>
            </td>
        </tr>
    </table>
</body>
</html>
""";

        email.Body = new TextPart(TextFormat.Html) { Text = body };

        using var smtp = new SmtpClient();
        try
        {
            await smtp.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(fromAddress, appPassword);
            await smtp.SendAsync(email);
        }
        finally
        {
            await smtp.DisconnectAsync(true);
        }
    }

    public async Task SendOrderConfirmationEmail(string to, string orderId, string orderTotal, bool isCod, bool isBalance = false)
    {
        var fromAddress = configuration["EmailSettings:From"];
        var appPassword = configuration["EmailSettings:AppPassword"];

        var email = new MimeMessage();
        email.From.Add(new MailboxAddress("Overclocked (No Reply)", fromAddress));
        email.ReplyTo.Add(MailboxAddress.Parse("no-reply@overclocked.com"));
        email.To.Add(MailboxAddress.Parse(to));
        email.Subject = $"Order Confirmation - #{orderId}";

        var paymentMessage = isCod
            ? "Your order has been placed. Payment will be collected upon delivery."
            : "Great news! Your payment has been authorized and your order is locked in.";

        var paymentMethod = isCod ? "Cash on Delivery" : "Credit/Debit Card";
        paymentMethod = isBalance ? "Balance" : paymentMethod;

        var body = $"""
<!DOCTYPE html>
<html lang="en">
<body style="font-family: 'Segoe UI', Arial, sans-serif; background-color: #f5f7fa; padding: 40px 0; margin: 0;">
    <table align="center" width="100%" style="max-width: 500px; background: #ffffff; border-radius: 8px; border: 1px solid #ddd; box-shadow: 0 2px 5px rgba(0,0,0,0.05);">

        <!-- Green Success Header -->
        <tr>
            <td style="padding: 20px; background-color: #2e7d32; border-radius: 8px 8px 0 0; text-align: center;">
                <h2 style="color: #ffffff; margin: 0; font-size: 22px;">Order Confirmed</h2>
            </td>
        </tr>

        <tr>
            <td style="padding: 30px 25px;">
                <p style="font-size: 16px; color: #333; margin-top: 0;">Hi there,</p>

                <p style="font-size: 15px; color: #333; line-height: 1.5;">
                    Thanks for shopping with Overclocked! {paymentMessage}
                </p>

                <!-- Order Summary Box -->
                <div style="background-color: #f8f9fa; border: 1px solid #e9ecef; padding: 15px; border-radius: 4px; margin: 20px 0;">
                    <table width="100%">
                        <tr>
                            <td style="color: #555; font-size: 14px;">Order ID:</td>
                            <td style="text-align: right; font-weight: bold; color: #333;">#{orderId}</td>
                        </tr>
                        <tr>
                            <td style="color: #555; font-size: 14px; padding-top: 8px;">Order Total:</td>
                            <td style="text-align: right; font-weight: bold; color: #2e7d32; padding-top: 8px;">{orderTotal}</td>
                        </tr>
                        <tr>
                            <td style="color: #555; font-size: 14px; padding-top: 8px;">Payment Method:</td>
                            <td style="text-align: right; color: #333; padding-top: 8px;">{paymentMethod}</td>
                        </tr>
                    </table>
                </div>

                <!-- The Grace Period Logic Note -->
                <p style="font-size: 13px; color: #666; font-style: italic; margin-top: 25px;">
                    <strong>Need to make changes?</strong><br>
                    You have 30 minutes to modify or cancel this order from your <a href="#" style="color: #0066c0; text-decoration: none;">Order History</a> before we begin processing it.
                </p>

                <hr style="border:none; border-top:1px solid #eee; margin: 25px 0;" />

                <div style="text-align: center;">
                    <a href="#" style="background-color: #f0c14b; border: 1px solid #a88734; color: #111; padding: 10px 20px; text-decoration: none; font-weight: bold; border-radius: 3px; display: inline-block; font-size: 14px;">
                        View Your Order
                    </a>
                </div>

                <p style="font-size: 12px; color: #999; text-align: center; margin-top: 20px;">
                    Overclocked - High Performance Gear
                </p>
            </td>
        </tr>
    </table>
</body>
</html>
""";

        email.Body = new TextPart(TextFormat.Html) { Text = body };

        using var smtp = new SmtpClient();
        try
        {
            await smtp.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(fromAddress, appPassword);
            await smtp.SendAsync(email);
        }
        finally
        {
            await smtp.DisconnectAsync(true);
        }
    }

    public async Task SendOrderRefundedEmail(string to, string orderId, string orderTotal, bool addToBalance)
    {
        var fromAddress = configuration["EmailSettings:From"];
        var appPassword = configuration["EmailSettings:AppPassword"];

        var email = new MimeMessage();
        email.From.Add(new MailboxAddress("Overclocked (No Reply)", fromAddress));
        email.ReplyTo.Add(MailboxAddress.Parse("no-reply@overclocked.com"));
        email.To.Add(MailboxAddress.Parse(to));
        email.Subject = $"Refund Processed - #{orderId}";

        // Logic to determine the text based on where the money went
        string destinationText = addToBalance
            ? "your Overclocked Wallet Balance"
            : "your original payment method (Credit/Debit Card)";

        string timeLineText = addToBalance
            ? "<strong>Good news!</strong> These funds are available immediately for your next purchase."
            : "Please allow <strong>5-10 business days</strong> for the credit to appear on your bank statement, depending on your bank's processing times.";

        var body = $"""
<!DOCTYPE html>
<html lang="en">
<body style="font-family: 'Segoe UI', Arial, sans-serif; background-color: #f5f7fa; padding: 40px 0; margin: 0;">
    <table align="center" width="100%" style="max-width: 500px; background: #ffffff; border-radius: 8px; border: 1px solid #ddd; box-shadow: 0 2px 5px rgba(0,0,0,0.05);">

        <!-- Blue Info Header -->
        <tr>
            <td style="padding: 20px; background-color: #1976d2; border-radius: 8px 8px 0 0; text-align: center;">
                <h2 style="color: #ffffff; margin: 0; font-size: 22px;">Refund Processed</h2>
            </td>
        </tr>

        <tr>
            <td style="padding: 30px 25px;">
                <p style="font-size: 16px; color: #333; margin-top: 0;">Hello,</p>

                <p style="font-size: 15px; color: #333; line-height: 1.5;">
                    We are writing to confirm that a refund has been issued for your order <strong>#{orderId}</strong>.
                </p>

                <!-- Refund Details Box -->
                <div style="background-color: #e3f2fd; border: 1px solid #bbdefb; padding: 15px; border-radius: 4px; margin: 20px 0;">
                    <table width="100%">
                        <tr>
                            <td style="color: #555; font-size: 14px;">Refund Amount:</td>
                            <td style="text-align: right; font-weight: bold; color: #1976d2;">{orderTotal}</td>
                        </tr>
                        <tr>
                            <td style="color: #555; font-size: 14px; padding-top: 8px;">Refunded To:</td>
                            <td style="text-align: right; color: #333; font-weight: bold; padding-top: 8px;">
                                {(addToBalance ? "Wallet Balance" : "Original Card")}
                            </td>
                        </tr>
                    </table>
                </div>

                <p style="font-size: 15px; color: #333; line-height: 1.5;">
                    The amount has been sent to {destinationText}.
                </p>

                <p style="font-size: 14px; color: #555; background-color: #f8f9fa; padding: 10px; border-radius: 4px;">
                    {timeLineText}
                </p>

                <hr style="border:none; border-top:1px solid #eee; margin: 25px 0;" />

                <p style="font-size: 13px; color: #666;">
                    If you do not see the refund after the time specified above, please reply to this email or contact support with your Order ID.
                </p>

                <p style="font-size: 12px; color: #999; text-align: center; margin-top: 20px;">
                    Overclocked - High Performance Gear
                </p>
            </td>
        </tr>
    </table>
</body>
</html>
""";

        email.Body = new TextPart(TextFormat.Html) { Text = body };

        using var smtp = new SmtpClient();
        try
        {
            await smtp.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(fromAddress, appPassword);
            await smtp.SendAsync(email);
        }
        finally
        {
            await smtp.DisconnectAsync(true);
        }
    }
}
