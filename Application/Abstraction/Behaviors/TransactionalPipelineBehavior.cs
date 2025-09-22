using System.Data;
using Application.Abstraction.Messaging;
using Application.Abstraction.Services;
using Microsoft.Extensions.Logging;

namespace Application.Abstraction.Behaviors;

public sealed class TransactionalPipelineBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ITransactionalCommand<TResponse>
{
    private readonly ILogger<TransactionalPipelineBehavior<TRequest, TResponse>> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public TransactionalPipelineBehavior(ILogger<TransactionalPipelineBehavior<TRequest, TResponse>> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        string requestName = request.GetType().Name;

        _logger.LogInformation("Beginning transaction for {RequestName}", requestName);

        using IDbTransaction transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken: cancellationToken);

        TResponse response = await next(cancellationToken);

        transaction.Commit();

        _logger.LogInformation("Committed transaction for {RequestName}", requestName);

        return response;
    }
}
