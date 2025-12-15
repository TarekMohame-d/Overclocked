namespace Overclocked.Infrastructure.Outbox;

public sealed record OutboxMessage
{
    public Guid Id { get; private set; }
    public string Type { get; private set; }
    public string Payload { get; private set; }
    public DateTime OccurredOnUtc { get; private set; }

    public DateTime? ProcessedOnUtc { get; private set; }
    public string? Error { get; private set; }
    public int RetryCount { get; private set; }

    public OutboxMessage(Guid id, string type, string payload, DateTime occurredOnUtc)
    {
        Id = id;
        Type = type;
        Payload = payload;
        OccurredOnUtc = occurredOnUtc;
        RetryCount = 0;
    }

    public void MarkProcessed()
    {
        ProcessedOnUtc = DateTime.UtcNow;
        Error = null;
    }

    public void HandleFailure(string error, int maxRetries)
    {
        RetryCount++;
        if(RetryCount >= maxRetries)
        {
            // Stop retrying, mark as "Dead Letter"
            ProcessedOnUtc = DateTime.UtcNow;
            Error = $"Failed after {maxRetries} attempts. Last error: {error}";
        }
    }
}
