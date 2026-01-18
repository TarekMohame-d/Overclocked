using Hangfire;
using Overclocked.Infrastructure.BackgroundJobs;

namespace Overclocked.Api.Extensions;

public static class BackgroundJobExtensions
{
    public static IApplicationBuilder UseBackgroundJobs(this WebApplication app)
    {
        IRecurringJobManager recurringJobManager = app.Services.GetRequiredService<IRecurringJobManager>();

        recurringJobManager.AddOrUpdate<ProcessOutboxMessagesJob>(
            "outbox-processor",
            job => job.ProcessOutboxMessagesAsync(),
            "*/15 * * * * *" // Run every 15 seconds
        );

        recurringJobManager.AddOrUpdate<OrderExpiryJob>(
            "order-expiry-processor",
            job => job.ProcessExpiredOrdersAsync(),
            Cron.Minutely // Run every minute
        );

        recurringJobManager.AddOrUpdate<OrderProcessingJob>(
            "order-processing-job",
            job => job.ProcessPlacedOrdersAsync(),
            Cron.MinuteInterval(5) // Run every 5 minutes to check for orders that passed the 30min mark
        );

        recurringJobManager.AddOrUpdate<ProcessPendingWebhooksJob>(
            "process-pending-webhooks",
            job => job.ProcessPendingWebhooksAsync(),
            "*/15 * * * * *" // Run every 15 seconds
        );

        return app;
    }
}
