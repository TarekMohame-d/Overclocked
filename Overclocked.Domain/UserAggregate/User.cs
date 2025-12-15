using System.ComponentModel.DataAnnotations.Schema;
using Overclocked.Domain.Common.Primitives;
using Overclocked.Domain.Common.StaticData;
using Overclocked.Domain.RoleAggregate;
using Overclocked.Domain.RoleAggregate.ValueObjects;
using Overclocked.Domain.UserAggregate.Entities;
using Overclocked.Domain.UserAggregate.Events;
using Overclocked.Domain.UserAggregate.ValueObjects;

namespace Overclocked.Domain.UserAggregate;

public class User : AggregateRoot<UserId>
{
    public RoleId RoleId { get; private set; }
    [NotMapped]
    public RoleType RoleType
    {
        get => (RoleType)RoleId.Value;
    }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string Email { get; private set; }
    public bool EmailConfirmed { get; private set; }
    public string PasswordHash { get; private set; }
    public string Phone { get; private set; }
    public bool IsActive { get; private set; }
    public EmailConfirmationCode EmailConfirmationCode { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private readonly List<RefreshToken> _refreshTokens = [];
    public IReadOnlyList<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();

    private readonly List<Address> _addresses = [];
    public IReadOnlyList<Address> Addresses => _addresses.AsReadOnly();

    public Role? Role { get; }

    private User()
    {
    }
    private User(
        UserId id,
        RoleId roleId,
        string firstName,
        string lastName,
        string email,
        string passwordHash,
        string phone,
        bool isActive = true) : base(id)
    {
        RoleId = roleId;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PasswordHash = passwordHash;
        Phone = phone;
        IsActive = isActive;

        EmailConfirmed = false;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public static User Create(
        UserId id,
        RoleId roleId,
        string firstName,
        string lastName,
        string email,
        string passwordHash,
        string phone,
        string code,
        string codeHash,
        bool isActive = true)
    {
        var user = new User(
            id: id,
            roleId: roleId,
            firstName: firstName,
            lastName: lastName,
            email: email,
            passwordHash: passwordHash,
            phone: phone,
            isActive: isActive);

        user.CreateEmailConfirmationCode(codeHash);
        user.RaiseDomainEvent(new UserRegisteredEvent(email, code));

        return user;
    }

    public void ConfirmEmail()
    {
        if(EmailConfirmed)
        {
            return;
        }

        EmailConfirmed = true;

        EmailConfirmationCode = EmailConfirmationCode.MarkAsUsed();

        RaiseDomainEvent(new UserEmailConfirmedEvent(Id.Value));
    }

    public void ResendEmailConfirmationCode(string code, string codeHash)
    {
        CreateEmailConfirmationCode(codeHash);
        RaiseDomainEvent(new UserEmailConfirmationCodeResendEvent(Email, code));
    }

    public DateTime CreateRefreshToken(string deviceId, string tokenHash)
    {
        RefreshToken? refreshToken = _refreshTokens.SingleOrDefault(x => x.DeviceId == deviceId);

        if(refreshToken is not null)
        {
            refreshToken.Update(tokenHash);
        }
        else
        {
            refreshToken = RefreshToken.Create(
                RefreshTokenId.Create(),
                deviceId,
                tokenHash);

            _refreshTokens.Add(refreshToken);
        }

        return refreshToken.ExpiredAt;
    }

    public void UpdatePassword(string passwordHash)
    {
        PasswordHash = passwordHash;
        UpdatedAt = DateTime.UtcNow;
        EmailConfirmed = true;

        EmailConfirmationCode = EmailConfirmationCode.MarkAsUsed();
    }

    public void CreateEmailConfirmationCode(string codeHash)
    {
        EmailConfirmationCode = EmailConfirmationCode.Create(codeHash, false, DateTime.UtcNow.AddMinutes(10));
    }

    public void ChangeRole(RoleType roleType)
    {
        RoleId = RoleId.Create((int)roleType);
        UpdatedAt = DateTime.UtcNow;
    }
}
