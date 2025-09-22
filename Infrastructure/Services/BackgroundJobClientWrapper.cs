using System.Linq.Expressions;
using Application.Abstraction.Services;
using Hangfire;

namespace Infrastructure.Services;

public class BackgroundJobClientWrapper : IBackgroundJobClientWrapper
{
    private readonly IBackgroundJobClient _backgroundJobClient;

    public BackgroundJobClientWrapper(IBackgroundJobClient backgroundJobClient)
    {
        _backgroundJobClient = backgroundJobClient;
    }

    public string Enqueue(Expression<Func<Task>> methodCall)
    {
        return _backgroundJobClient.Enqueue(methodCall);
    }

    public string ContinueJobWith(string parentId, Expression<Func<Task>> methodCall)
    {
        // Hangfire provides a static API for continuations.
        // Use BackgroundJob.ContinueJobWith to create a continuation.
        return _backgroundJobClient.ContinueJobWith(parentId, methodCall);
    }
}
