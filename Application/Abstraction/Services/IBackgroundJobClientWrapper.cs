using System.Linq.Expressions;

namespace Application.Abstraction.Services;

public interface IBackgroundJobClientWrapper
{
    // Enqueue an async job and return the new job id
    string Enqueue(Expression<Func<Task>> methodCall);

    // Create a continuation job that runs after parentId completes
    string ContinueJobWith(string parentId, Expression<Func<Task>> methodCall);
}
