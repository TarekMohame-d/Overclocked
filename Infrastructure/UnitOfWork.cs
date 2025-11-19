using System.Data;
using Application.Abstraction.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infrastructure;

public class UnitOfWork(ApplicationDbContext context) : IUnitOfWork, IAsyncDisposable
{
    private IDbContextTransaction? _transaction;

    public async ValueTask DisposeAsync()
    {
        if(_transaction is not null)
            await _transaction.DisposeAsync();

        await context.DisposeAsync();

        GC.SuppressFinalize(this);
    }

    public async Task<IDbTransaction> BeginTransactionAsync(
        IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
        CancellationToken cancellationToken = default
    )
    {
        IDbContextTransaction transaction = await context.Database.BeginTransactionAsync(
            isolationLevel,
            cancellationToken
        );
        _transaction = transaction;

        return transaction.GetDbTransaction();
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if(_transaction is not null)
                await _transaction.CommitAsync(cancellationToken);
        }
        finally
        {
            if(_transaction is not null)
                await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if(_transaction is not null)
                await _transaction.RollbackAsync(cancellationToken);
        }
        finally
        {
            if(_transaction is not null)
                await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task<int> CompleteAsync(CancellationToken cancellationToken = default) =>
        await context.SaveChangesAsync(cancellationToken);
}
