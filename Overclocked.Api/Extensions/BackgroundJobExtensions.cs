using Hangfire;
using Overclocked.Infrastructure.Outbox;

namespace Overclocked.Api.Extensions;

public static class BackgroundJobExtensions
{
    public static IApplicationBuilder UseBackgroundJobs(this WebApplication app)
    {
        app.Services
            .GetRequiredService<IRecurringJobManager>()
            .AddOrUpdate<ProcessOutboxMessagesJob>(
                "outbox-processor",
                job => job.ProcessOutboxMessages(),
                "*/15 * * * * *"); // every 15 seconds

        return app;
    }
}
