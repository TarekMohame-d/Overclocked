using System.Data;

namespace Application.Abstraction.Messaging;

public interface ITransactionalRequest : IRequest
{
    IsolationLevel IsolationLevel => IsolationLevel.ReadCommitted;
}
