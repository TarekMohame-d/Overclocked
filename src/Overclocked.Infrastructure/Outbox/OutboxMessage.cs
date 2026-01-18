namespace Overclocked.Infrastructure.Outbox;

public sealed record OutboxMessage
{
    public Guid Id { get; private set; }
    public string Type { get; private set; }
    public string Payload { get; private set; }
    public DateTimeOffset OccurredOnUtc { get; private set; }

    public DateTimeOffset? ProcessedOnUtc { get; private set; }
    public string? Error { get; private set; }
    public int RetryCount { get; private set; }

    public OutboxMessage(Guid id, string type, string payload, DateTimeOffset occurredOnUtc)
    {
        Id = id;
        Type = type;
        Payload = payload;
        OccurredOnUtc = occurredOnUtc;
        RetryCount = 0;
    }

    public void MarkProcessed()
    {
        ProcessedOnUtc = DateTimeOffset.UtcNow;
        Error = null;
    }

    public void HandleFailure(string error, int maxRetries)
    {
        RetryCount++;
        if (RetryCount < maxRetries)
            return;
        // Stop retrying, mark as "Dead Letter"
        ProcessedOnUtc = DateTimeOffset.UtcNow;
        Error = $"Failed after {maxRetries} attempts. Last error: {error}";
    }
}
