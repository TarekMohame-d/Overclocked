using Microsoft.EntityFrameworkCore;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Domain.OrderAggregate.ValueObjects;
using Overclocked.Domain.PaymentAggregate;
using Overclocked.Domain.PaymentAggregate.ValueObjects;

namespace Overclocked.Infrastructure.Persistence.Repositories;

public class PaymentRepository(ApplicationDbContext dbContext) : IPaymentRepository
{
    private readonly DbSet<Payment> _dbSet = dbContext.Payments;

    public async Task<Payment?> GetByIdAsync(PaymentId id, CancellationToken ct = default) => await _dbSet.FindAsync([id], ct);

    public Task<Payment?> GetByOrderIdAsync(OrderId id, CancellationToken ct = default) =>
        _dbSet.AsTracking().FirstOrDefaultAsync(x => x.OrderId == id, ct);

    public Task<List<Payment>> GetByIdsAsync(List<OrderId> ids, CancellationToken ct = default) =>
        _dbSet.AsTracking().Where(p => ids.Contains(p.OrderId)).ToListAsync(ct);

    public void Add(Payment payment) => _dbSet.Add(payment);
}
