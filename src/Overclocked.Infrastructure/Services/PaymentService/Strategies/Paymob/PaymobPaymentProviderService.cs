using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Overclocked.Application.Abstractions.Services;
using Overclocked.Application.Common.Configurations;
using Overclocked.Application.Common.Constants.Payment;
using Overclocked.Domain.OrderAggregate;
using Overclocked.Domain.UserAggregate;
using Overclocked.Infrastructure.Inbox;
using Overclocked.Infrastructure.Persistence;
using Overclocked.SharedKernel;

namespace Overclocked.Infrastructure.Services.PaymentService.Strategies.Paymob;

public class PaymobPaymentProviderService(
    IHttpClientFactory httpClientFactory,
    IOptions<PaymobSettings> options,
    ApplicationDbContext dbContext,
    ILogger<PaymobPaymentProviderService> logger
) : IPaymentProviderService
{
    private readonly PaymobSettings _paymobSettings = options.Value;
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("PaymobClient");

    public PaymentProvider PaymentProvider => PaymentProvider.Paymob;

    public async Task<Result<string>> GeneratePaymentUrl(Order order, User user, PaymentMethod method, CancellationToken ct)
    {
        Result<int> integrationIdResult = GetPaymentIntegrationId(method, _paymobSettings);

        if (integrationIdResult.IsFailure)
            return Result.Failure<string>(integrationIdResult.Error);

        var payload = new
        {
            amount = decimal.ToInt32(order.TotalPrice.Value * 100),
            currency = "EGP",
            payment_methods = new[] { integrationIdResult.Value },
            items = order.Items.Select(x => new
            {
                name = x.ProductName,
                amount = decimal.ToInt32(x.UnitPrice.Value * 100),
                quantity = x.Quantity,
            }),
            billing_data = new
            {
                apartment = order.ShippingAddress.Apartment,
                building = order.ShippingAddress.Building,
                street = order.ShippingAddress.Street,
                country = order.ShippingAddress.City,
                first_name = user.FirstName,
                last_name = user.LastName,
                phone_number = user.Phone,
            },
            customer = new
            {
                first_name = user.FirstName,
                last_name = user.LastName,
                email = user.Email,
                phone_number = user.Phone,
            },
            extras = new
            {
                customer_id = user.Id.Value.ToString(),
                order_id = order.Id.Value.ToString(),
                payment_method = method.ToString(),
            },
        };

        var requestUrl = _paymobSettings.BaseUrl + "/v1/intention/";
        var request = new HttpRequestMessage(HttpMethod.Post, requestUrl);
        request.Headers.Authorization = new AuthenticationHeaderValue("Token", _paymobSettings.SecretKey);
        request.Content = JsonContent.Create(payload);

        HttpResponseMessage response = await _httpClient.SendAsync(request, ct);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            return Result.Failure<string>(Error.Failure("Paymob.Payment", $"Failed: {response.StatusCode} - {errorContent}"));
        }

        var responseJson = await response.Content.ReadFromJsonAsync<JsonElement>();

        if (!responseJson.TryGetProperty("client_secret", out JsonElement clientSecretElement))
            return Result.Failure<string>(Error.Failure("Paymob.Response", "Client secret not found in response"));

        var clientSecret = clientSecretElement.GetString();
        var redirectUrl =
            $"{_paymobSettings.BaseUrl}/unifiedcheckout/?publicKey={_paymobSettings.PublicKey}&clientSecret={clientSecret}";

        return Result.Success(redirectUrl);
    }

    public async Task<Result> ProcessCallback(string rawBody, IHeaderDictionary headers, IQueryCollection queryParams)
    {
        if (!queryParams.TryGetValue("hmac", out StringValues hmacValues))
        {
            logger.LogError("Paymob Callback: HMAC signature missing from query parameters.");
            return Result.Failure(Error.Failure("Paymob.Security", "HMAC signature missing from query parameters."));
        }

        using var document = JsonDocument.Parse(rawBody);

        if (!document.RootElement.TryGetProperty("obj", out JsonElement objElement))
        {
            logger.LogError("Paymob Callback: Could not find 'obj' element in request body.");
            return Result.Failure(Error.Failure("Paymob.Callback", "Could not find 'obj' element in request body."));
        }

        var isValid = PaymobValidator.ValidateProcessedCallback(_paymobSettings.Hmac, objElement, hmacValues.ToString());

        if (!isValid)
        {
            logger.LogError("Paymob Callback: HMAC verification failed. Data integrity compromised.");
            return Result.Failure(Error.Failure("Paymob.Security", "HMAC verification failed. Data integrity compromised."));
        }

        var transId = objElement.GetProperty("id").ToString();

        var webhookLog = new PaymentWebhook(transId, rawBody, DateTimeOffset.UtcNow);

        dbContext.Set<PaymentWebhook>().Add(webhookLog);
        await dbContext.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result> RefundPaymentAsync(string transactionId, decimal refundValue, CancellationToken ct)
    {
        var refundUrl = $"{_paymobSettings.BaseUrl}/api/acceptance/void_refund/refund";

        var payload = new { transaction_id = transactionId, amount_cents = decimal.ToInt32(refundValue * 100) };

        var request = new HttpRequestMessage(HttpMethod.Post, refundUrl);

        request.Headers.Authorization = new AuthenticationHeaderValue("Token", _paymobSettings.SecretKey);
        request.Content = JsonContent.Create(payload);

        try
        {
            HttpResponseMessage response = await _httpClient.SendAsync(request, ct);

            var responseJson = await response.Content.ReadFromJsonAsync<JsonElement>();

            if (!response.IsSuccessStatusCode)
            {
                string errorMessage = response.StatusCode.ToString();
                if (responseJson.TryGetProperty("message", out JsonElement message))
                {
                    errorMessage = message.GetString() ?? errorMessage;
                }

                logger.LogError("Paymob Refund Failed: {StatusCode} - {Message}", response.StatusCode, errorMessage);
                return Result.Failure(Error.Failure("Paymob.Refund", $"Gateway Error: {errorMessage}"));
            }

            if (responseJson.TryGetProperty("success", out JsonElement successEl) && successEl.GetBoolean())
            {
                logger.LogInformation("Paymob Refund Successful for Transaction {TransactionId}", transactionId);
                return Result.Success();
            }

            logger.LogWarning(
                "Paymob Refund Declined for Transaction {TransactionId}. Response: {Response}",
                transactionId,
                responseJson.ToString()
            );

            return Result.Failure(Error.Failure("Paymob.Refund", "Refund was declined by the gateway."));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception during Paymob refund for Transaction {TransactionId}", transactionId);
            return Result.Failure(
                Error.Failure("Paymob.Exception", "An error occurred while communicating with the payment gateway.")
            );
        }
    }

    private static Result<int> GetPaymentIntegrationId(PaymentMethod method, PaymobSettings settings)
    {
        var integrationId = method switch
        {
            PaymentMethod.CreditCard => settings.CardIntegrationId,
            PaymentMethod.EWallet => settings.EWalletIntegrationId,
            PaymentMethod.CashOnDelivery or _ => -1,
        };

        if (integrationId == -1)
            return Result.Failure<int>(Error.BadRequest("Payment.Method", "Invalid payment method"));

        return Result.Success(integrationId);
    }
}
