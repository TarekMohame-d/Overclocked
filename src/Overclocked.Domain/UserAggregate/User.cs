using System.Text.RegularExpressions;
using Overclocked.Domain.Common.Shared.ValueObjects.Address;
using Overclocked.Domain.Common.Shared.ValueObjects.Money;
using Overclocked.Domain.UserAggregate.Entities;
using Overclocked.Domain.UserAggregate.Enums;
using Overclocked.Domain.UserAggregate.Events;
using Overclocked.Domain.UserAggregate.ValueObjects;
using Overclocked.SharedKernel;
using Overclocked.SharedKernel.Primitives;

namespace Overclocked.Domain.UserAggregate;

public sealed class User : AggregateRoot<UserId>
{
    public Role Role { get; private set; }
    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public bool EmailConfirmed { get; private set; }
    public string PasswordHash { get; private set; } = null!;
    public string Phone { get; private set; } = null!;
    public Money Balance { get; private set; } = null!;
    public bool IsActive { get; private set; }
    public EmailConfirmationCode EmailConfirmationCode { get; private set; } = null!;
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    private readonly List<RefreshToken> _refreshTokens = [];
    public IReadOnlyList<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();

    private readonly List<Address> _addresses = [];
    public IReadOnlyList<Address> Addresses => _addresses.AsReadOnly();

    private User() { }

    private User(UserId id, string firstName, string lastName, string email, string passwordHash, string phone)
        : base(id)
    {
        Role = Role.Customer;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PasswordHash = passwordHash;
        Phone = phone;
        Balance = Money.Zero;

        IsActive = true;
        EmailConfirmed = false;

        DateTimeOffset now = DateTimeOffset.UtcNow;
        CreatedAt = now;
        UpdatedAt = now;
    }

    public static Result<User> Create(
        string firstName,
        string lastName,
        string email,
        string passwordHash,
        string phone,
        string code,
        string codeHash
    )
    {
        Result validationResult = ValidateState(firstName, lastName, email, phone);
        if (validationResult.IsFailure)
            return Result.Failure<User>(validationResult.Error);

        var user = new User(UserId.Create(), firstName, lastName, email, passwordHash, phone);

        user.CreateEmailConfirmationCode(codeHash);
        user.RaiseDomainEvent(new UserRegisteredEvent(email, code));

        return Result.Success(user);
    }

    public void AddToBalance(Money amount)
    {
        Balance += amount;
        UpdatedAt = DateTimeOffset.UtcNow;

        RaiseDomainEvent(new PaymentRefundEvent(Balance.Value));
    }

    public void RemoveFromBalance(Money amount)
    {
        Balance -= amount;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void ConfirmEmail()
    {
        if (EmailConfirmed)
            return;

        EmailConfirmed = true;

        EmailConfirmationCode = EmailConfirmationCode.MarkAsUsed();

        RaiseDomainEvent(new UserEmailConfirmedEvent(Id.Value));
    }

    public void ResendEmailConfirmationCode(string code, string codeHash)
    {
        CreateEmailConfirmationCode(codeHash);
        RaiseDomainEvent(new UserEmailConfirmationCodeResendEvent(Email, code));
    }

    public Result<DateTimeOffset> CreateRefreshToken(Guid deviceId, string tokenHash)
    {
        RefreshToken? refreshToken = _refreshTokens.SingleOrDefault(x => x.DeviceId == deviceId);

        if (refreshToken is not null)
        {
            Result result = refreshToken.Update(tokenHash);

            if (result.IsFailure)
                return Result.Failure<DateTimeOffset>(result.Error);
        }
        else
        {
            Result<RefreshToken> result = RefreshToken.Create(deviceId, tokenHash);

            if (result.IsFailure)
                return Result.Failure<DateTimeOffset>(result.Error);

            refreshToken = result.Value;

            _refreshTokens.Add(refreshToken);
        }

        return Result.Success(refreshToken.ExpiredAt);
    }

    public void UpdatePassword(string passwordHash)
    {
        PasswordHash = passwordHash;
        UpdatedAt = DateTimeOffset.UtcNow;
        EmailConfirmed = true;

        EmailConfirmationCode = EmailConfirmationCode.MarkAsUsed();
    }

    public void CreateEmailConfirmationCode(string codeHash) => EmailConfirmationCode = EmailConfirmationCode.Create(codeHash);

    public void ChangeRole(Role role)
    {
        Role = role;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void Deactivate() => IsActive = false;

    public Result AddAddress(int apartment, string building, string street, string city, string postalCode, string description)
    {
        Result<Address> result = Address.Create(apartment, building, street, city, postalCode, description);

        if (result.IsFailure)
            return Result.Failure(result.Error);

        Address address = result.Value;

        if (_addresses.Contains(result.Value))
            return Result.Success();

        _addresses.Add(address);
        UpdatedAt = DateTimeOffset.UtcNow;

        return Result.Success();
    }

    public Result RemoveAddress(int apartment, string building, string street, string city, string postalCode, string description)
    {
        Result<Address> result = Address.Create(apartment, building, street, city, postalCode, description);

        if (result.IsFailure)
            return Result.Failure(result.Error);

        Address address = result.Value;

        _addresses.Remove(address);
        UpdatedAt = DateTimeOffset.UtcNow;

        return Result.Success();
    }

    private static Result ValidateState(string firstName, string lastName, string email, string phone)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            return Result.Failure(UserErrors.FirstNameIsRequired);

        if (firstName.Length > 20)
            return Result.Failure(UserErrors.FirstNameIsTooLong);

        if (string.IsNullOrWhiteSpace(lastName))
            return Result.Failure(UserErrors.LastNameIsRequired);

        if (lastName.Length > 20)
            return Result.Failure(UserErrors.LastNameIsTooLong);

        if (string.IsNullOrWhiteSpace(email))
            return Result.Failure(UserErrors.EmailIsRequired);

        var regex = new Regex(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$");

        if (!(regex?.IsMatch(email) ?? false))
            return Result.Failure(UserErrors.InvalidEmail);

        if (string.IsNullOrWhiteSpace(phone))
            return Result.Failure(UserErrors.PhoneIsRequired);

        regex = new Regex(@"^\+?\d{10,15}$");

        if (!(regex?.IsMatch(phone) ?? false))
            return Result.Failure(UserErrors.InvalidPhone);

        return Result.Success();
    }
}
