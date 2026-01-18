using Overclocked.SharedKernel.Primitives;

namespace Overclocked.Domain.UserAggregate.ValueObjects;

public record EmailConfirmationCode : IValueObject
{
    private EmailConfirmationCode() { }

    private EmailConfirmationCode(string codeHash, bool isUsed, DateTimeOffset expiredAt)
    {
        CodeHash = codeHash;
        IsUsed = isUsed;
        ExpiredAt = expiredAt;
    }

    public string CodeHash { get; private set; } = null!;
    public bool IsUsed { get; private set; }
    public DateTimeOffset ExpiredAt { get; private set; }

    public static EmailConfirmationCode Create(string codeHash) => new(codeHash, false, DateTimeOffset.UtcNow.AddMinutes(10));

    public EmailConfirmationCode MarkAsUsed() => new(CodeHash, true, ExpiredAt);
}
