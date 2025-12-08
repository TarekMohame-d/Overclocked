using Overclocked.Domain.Common.Primitives;

namespace Overclocked.Domain.UserAggregate.ValueObjects;

public record EmailConfirmationCode : IValueObject
{
    public string CodeHash { get; private set; }
    public bool IsUsed { get; private set; }
    public DateTime ExpiredAt { get; private set; }

    private EmailConfirmationCode()
    {
    }
    private EmailConfirmationCode(string codeHash, bool isUsed, DateTime expiredAt)
    {
        CodeHash = codeHash;
        IsUsed = isUsed;
        ExpiredAt = expiredAt;
    }

    public static EmailConfirmationCode Create(string codeHash, bool isUsed, DateTime expiredAt)
    {
        return new(codeHash, isUsed, expiredAt);
    }

    public EmailConfirmationCode MarkAsUsed()
    {
        return new(CodeHash, true, ExpiredAt);
    }
}
