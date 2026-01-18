using Overclocked.Domain.OrderAggregate.ValueObjects;
using Overclocked.Domain.PaymentAggregate;
using Overclocked.Domain.PaymentAggregate.ValueObjects;

namespace Overclocked.Application.Abstractions.Persistence;

public interface IPaymentRepository : IRepository
{
    Task<Payment?> GetByIdAsync(PaymentId id, CancellationToken ct = default);
    Task<Payment?> GetByOrderIdAsync(OrderId id, CancellationToken ct = default);
    Task<List<Payment>> GetByIdsAsync(List<OrderId> ids, CancellationToken ct = default);
    void Add(Payment payment);
}
