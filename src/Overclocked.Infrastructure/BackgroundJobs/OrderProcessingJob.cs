using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Domain.OrderAggregate;

namespace Overclocked.Infrastructure.BackgroundJobs;

public class OrderProcessingJob(IServiceScopeFactory scopeFactory, ILogger<OrderProcessingJob> logger)
{
    private readonly TimeSpan _gracePeriod = TimeSpan.FromMinutes(30);
    private const int BatchSize = 50;

    [DisableConcurrentExecution(timeoutInSeconds: 10 * 60)]
    [AutomaticRetry(Attempts = 3)]
    public async Task ProcessPlacedOrdersAsync()
    {
        await using AsyncServiceScope scope = scopeFactory.CreateAsyncScope();

        IOrderRepository orderRepository = scope.ServiceProvider.GetRequiredService<IOrderRepository>();
        IUnitOfWork unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        DateTimeOffset cutoffTime = DateTimeOffset.UtcNow.Subtract(_gracePeriod);

        List<Order> placedOrders = await orderRepository.GetPlacedOrdersOlderThanAsync(
            cutoffTime,
            BatchSize,
            CancellationToken.None
        );

        if (!placedOrders.Any())
            return;

        logger.LogInformation("Found {Count} orders ready for processing. Locking orders...", placedOrders.Count);

        foreach (Order order in placedOrders)
        {
            logger.LogInformation("Moving Order {OrderId} to Processing state", order.Id.Value);
            order.MarkAsProcessing();
        }

        await unitOfWork.SaveChangesAsync(CancellationToken.None);
    }
}
