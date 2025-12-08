namespace Overclocked.Infrastructure.Outbox;

public sealed record OutboxMessage
{
    public Guid Id { get; private set; }
    public string Type { get; private set; }
    public string Payload { get; private set; }
    public DateTime OccurredOnUtc { get; private set; }

    public DateTime? ProcessedOnUtc { get; private set; }
    public string? Error { get; private set; }

    public OutboxMessage(Guid id, string type, string payload, DateTime occurredOnUtc)
    {
        Id = id;
        Type = type;
        Payload = payload;
        OccurredOnUtc = occurredOnUtc;
    }

    public void MarkProcessed() => ProcessedOnUtc = DateTime.UtcNow;

    public void MarkFailed(string error) => Error = error;
}
