namespace Overclocked.Application.Abstractions.Services;

public interface IEmailService
{
    Task SendConfirmationCode(string to, string code);
    Task SendOrderCancellationEmail(string to, string orderId);
    Task SendPaymentFailedEmail(string to, string orderId, string orderTotal);
    Task SendOrderConfirmationEmail(string to, string orderId, string orderTotal, bool isCod, bool isBalance = false);
    Task SendOrderRefundedEmail(string to, string orderId, string orderTotal, bool addToBalance);
}
