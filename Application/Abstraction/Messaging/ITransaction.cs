using System.Data;

namespace Application.Abstraction.Messaging;

public interface ITransaction : IRequest
{
    IsolationLevel IsolationLevel => IsolationLevel.ReadCommitted;
}
