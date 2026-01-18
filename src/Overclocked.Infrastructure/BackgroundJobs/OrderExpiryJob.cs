using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Domain.OrderAggregate;
using Overclocked.Domain.OrderAggregate.Entities;
using Overclocked.Domain.PaymentAggregate;
using Overclocked.Domain.ProductAggregate;

namespace Overclocked.Infrastructure.BackgroundJobs;

public class OrderExpiryJob(IServiceScopeFactory scopeFactory, ILogger<OrderExpiryJob> logger)
{
    private readonly TimeSpan _expiryDuration = TimeSpan.FromMinutes(30);
    private const int BatchSize = 50;

    [DisableConcurrentExecution(timeoutInSeconds: 10 * 60)]
    [AutomaticRetry(Attempts = 3)]
    public async Task ProcessExpiredOrdersAsync()
    {
        logger.LogInformation("Starting expired orders processing job");
        await using AsyncServiceScope scope = scopeFactory.CreateAsyncScope();

        IOrderRepository orderRepository = scope.ServiceProvider.GetRequiredService<IOrderRepository>();
        IProductRepository productRepository = scope.ServiceProvider.GetRequiredService<IProductRepository>();
        IPaymentRepository paymentRepository = scope.ServiceProvider.GetRequiredService<IPaymentRepository>();
        IUnitOfWork unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        DateTimeOffset cutoffTime = DateTimeOffset.UtcNow.Subtract(_expiryDuration);

        List<Order> expiredOrders = await orderRepository.GetPendingOrdersOlderThanAsync(
            cutoffTime,
            BatchSize,
            CancellationToken.None
        );

        if (!expiredOrders.Any())
        {
            logger.LogInformation("No expired orders found");
            return;
        }

        logger.LogInformation("Found {Count} expired orders. Processing cancellation...", expiredOrders.Count);

        var allProductIds = expiredOrders.SelectMany(o => o.Items).Select(i => i.ProductId).Distinct().ToList();

        List<Product> products = await productRepository.GetByIdsAsync(allProductIds, CancellationToken.None);
        var productsById = products.ToDictionary(p => p.Id);

        foreach (Order order in expiredOrders)
        {
            try
            {
                logger.LogInformation("Cancelling Order {OrderId}", order.Id.Value);

                order.MarkAsCancelled();

                Payment? payment = await paymentRepository.GetByOrderIdAsync(order.Id, CancellationToken.None);
                payment?.MarkAsCancelled();

                foreach (OrderItem item in order.Items)
                {
                    if (productsById.TryGetValue(item.ProductId, out Product? product))
                    {
                        product.AddStock(item.Quantity);
                    }
                    else
                    {
                        logger.LogError(
                            "Critical: Product {ProductId} not found while restocking Order {OrderId}",
                            item.ProductId,
                            order.Id
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to process expiry for Order {OrderId}", order.Id.Value);
            }
        }

        await unitOfWork.SaveChangesAsync(CancellationToken.None);
    }
}
