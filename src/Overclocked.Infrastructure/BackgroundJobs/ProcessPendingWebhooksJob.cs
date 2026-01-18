using System.Text.Json;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Overclocked.Application.Abstractions.Services;
using Overclocked.Domain.Common.Shared.ValueObjects.Money;
using Overclocked.Domain.OrderAggregate.ValueObjects;
using Overclocked.Domain.UserAggregate.ValueObjects;
using Overclocked.Infrastructure.Inbox;
using Overclocked.Infrastructure.Persistence;
using Overclocked.Infrastructure.Services.PaymentService.Strategies.Paymob;
using Overclocked.SharedKernel;

namespace Overclocked.Infrastructure.BackgroundJobs;

public class ProcessPendingWebhooksJob(IServiceScopeFactory scopeFactory, ILogger<ProcessPendingWebhooksJob> logger)
{
    private const int BatchSize = 50;
    private const int MaxRetries = 3;
    private static readonly JsonSerializerOptions _serializerSettings = new() { PropertyNameCaseInsensitive = true };

    [DisableConcurrentExecution(timeoutInSeconds: 0)]
    [AutomaticRetry(Attempts = 3, OnAttemptsExceeded = AttemptsExceededAction.Fail)]
    public async Task ProcessPendingWebhooksAsync()
    {
        logger.LogInformation("Beginning to process Pending Webhooks");
        await using AsyncServiceScope scope = scopeFactory.CreateAsyncScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        IPaymentService paymentService = scope.ServiceProvider.GetRequiredService<IPaymentService>();

        List<PaymentWebhook> paymentsWebhooks = await dbContext
            .Set<PaymentWebhook>()
            .AsTracking()
            .Where(m => m.ProcessedOnUtc == null && m.RetryCount < MaxRetries)
            .OrderBy(m => m.CreatedOnUtc)
            .Take(BatchSize)
            .ToListAsync();

        if (paymentsWebhooks.Count == 0)
        {
            logger.LogInformation("No pending webhooks found");
            return;
        }

        foreach (var webhook in paymentsWebhooks)
        {
            try
            {
                var callbackPayload = JsonSerializer.Deserialize<PaymobCallbackRoot>(webhook.Payload, _serializerSettings);

                if (callbackPayload is null)
                {
                    logger.LogError("Paymob Callback: Failed to deserialize callback data.");
                    webhook.HandleFailure("Failed to deserialize callback data.", MaxRetries);
                    continue;
                }

                Guid orderId = callbackPayload.Obj.Claims?.Extra?.OrderId ?? Guid.Empty;
                Guid userId = callbackPayload.Obj.Claims?.Extra?.CustomerId ?? Guid.Empty;

                if (orderId == Guid.Empty || userId == Guid.Empty)
                {
                    logger.LogError(
                        "Paymob Callback: Failed to parse order id or user id. OrderId: {OrderId}, CustomerId: {CustomerId}",
                        orderId,
                        userId
                    );
                    webhook.HandleFailure(
                        $"Failed to parse order id or user id. OrderId: {orderId}, CustomerId: {userId}.",
                        MaxRetries
                    );
                    continue;
                }

                if (callbackPayload.Obj.IsRefunded && callbackPayload.Obj.Success) // Handle Refund
                {
                    logger.LogInformation("Paymob Callback: Transaction {TransactionId} is Refunded.", webhook.TransactionId);

                    Result<Money> amountResult = Money.Create(callbackPayload.Obj.AmountCents / 100);
                    if (amountResult.IsFailure)
                    {
                        logger.LogCritical("Paymob Callback: Failed to parse refund amount. {@error}", amountResult.Error);
                        throw new Exception("Refund Money Parse Failed");
                    }

                    await paymentService.HandleRefundAsync(
                        OrderId.Create(orderId),
                        UserId.Create(userId),
                        webhook.TransactionId,
                        amountResult.Value
                    );
                }
                else if (callbackPayload.Obj.Success) // Handle Success Payment
                {
                    logger.LogInformation("Paymob Callback: Payment succeeded.");

                    await paymentService.HandleSuccessfulPaymentAsync(
                        OrderId.Create(orderId),
                        UserId.Create(userId),
                        webhook.TransactionId
                    );
                }
                else // Handle Failure
                {
                    logger.LogInformation("Paymob Callback: Payment failed.");

                    await paymentService.HandleFailedPaymentAsync(
                        OrderId.Create(orderId),
                        UserId.Create(userId),
                        webhook.TransactionId
                    );
                }

                webhook.MarkProcessed();
                logger.LogInformation("Webhook {Id} processed successfully.", webhook.Id);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing webhook {Id}", webhook.Id);
                webhook.HandleFailure(ex.ToString(), MaxRetries);
            }
        }

        await dbContext.SaveChangesAsync();
    }
}
