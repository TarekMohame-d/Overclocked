namespace Overclocked.Infrastructure.Inbox;

public sealed record PaymentWebhook
{
    public Guid Id { get; private set; }
    public string TransactionId { get; private set; }
    public string Payload { get; private set; }
    public DateTimeOffset CreatedOnUtc { get; private set; }
    public DateTimeOffset? ProcessedOnUtc { get; private set; }
    public string? ErrorLog { get; private set; }
    public int RetryCount { get; private set; }

    public PaymentWebhook(string transactionId, string payload, DateTimeOffset createdOnUtc)
    {
        Id = Guid.CreateVersion7();
        TransactionId = transactionId;
        Payload = payload;
        CreatedOnUtc = createdOnUtc;
        RetryCount = 0;
    }

    public void MarkProcessed()
    {
        ProcessedOnUtc = DateTimeOffset.UtcNow;
        ErrorLog = null;
    }

    public void HandleFailure(string error, int maxRetries)
    {
        RetryCount++;
        if (RetryCount < maxRetries)
            return;
        // Stop retrying, mark as "Dead Letter"
        ProcessedOnUtc = DateTimeOffset.UtcNow;
        ErrorLog = $"Failed after {maxRetries} attempts. Last error: {error}";
    }
}
