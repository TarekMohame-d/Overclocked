using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Overclocked.Api.Routing;
using Overclocked.Architecture.Tests.FakeData;
using Overclocked.Domain.CartAggregate;
using Overclocked.Domain.UserAggregate;
using Overclocked.Domain.UserAggregate.Events;
using Overclocked.Domain.WishlistAggregate;
using Overclocked.Infrastructure.Authentication;
using Overclocked.Infrastructure.Outbox;
using Overclocked.Infrastructure.Persistence;
using Overclocked.Integration.Tests.Shared;
using Overclocked.SharedKernel.Primitives;
using Shouldly;

namespace Overclocked.Integration.Tests.AuthenticationTests;

public class ConfirmEmailTest(ApiTestFixture fixture) : IAsyncLifetime
{
    private const string PlainCode = "VC4R53";
    private readonly HttpClient _client = fixture.HttpClient;

    public async ValueTask InitializeAsync() => await fixture.ResetDatabaseAsync();

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    [Fact]
    public async Task ConfirmEmail_Should_ReturnSuccess_When_DataIsValid()
    {
        // Arrange
        User user = await SeedDatabaseAsync();
        StringContent form = CreateJsonContent(user.Email);

        // Act
        HttpResponseMessage response = await _client.PostAsync(AuthRoutes.ConfirmEmail, form);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        await using AsyncServiceScope scope = fixture.Services.CreateAsyncScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        User? userDb = await dbContext
            .Users.Include(u => u.EmailConfirmationCode)
            .FirstOrDefaultAsync(x => x.Email == user.Email);

        userDb.ShouldNotBeNull();
        userDb.EmailConfirmed.ShouldBeTrue();
        userDb.EmailConfirmationCode.IsUsed.ShouldBeTrue();

        OutboxMessage? message = await dbContext
            .Set<OutboxMessage>()
            .FirstOrDefaultAsync(x => x.Type == nameof(UserEmailConfirmedEvent));

        message.ShouldNotBeNull();
        message.ProcessedOnUtc.ShouldBeNull();

        var domainEvent = new UserEmailConfirmedEvent(userDb.Id.Value);

        IDomainEventHandler<UserEmailConfirmedEvent> handler = scope.ServiceProvider.GetRequiredService<
            IDomainEventHandler<UserEmailConfirmedEvent>
        >();

        await handler.Handle(domainEvent, CancellationToken.None);
        await dbContext.SaveChangesAsync();

        dbContext.ChangeTracker.Clear();

        Cart? cart = await dbContext.Carts.FirstOrDefaultAsync(c => c.UserId == userDb.Id);
        Wishlist? wishlist = await dbContext.Wishlists.FirstOrDefaultAsync(w => w.UserId == userDb.Id);

        cart.ShouldNotBeNull();
        cart.UserId.ShouldBe(userDb.Id);

        wishlist.ShouldNotBeNull();
        wishlist.UserId.ShouldBe(userDb.Id);
    }

    private async Task<User> SeedDatabaseAsync()
    {
        var emailConfirmationCodeService = new EmailConfirmationCodeService();
        var codeHash = emailConfirmationCodeService.Hash(PlainCode);
        User user = new UserFaker(new PasswordHasher()).Generate();
        user.CreateEmailConfirmationCode(codeHash);

        await using AsyncServiceScope scope = fixture.Services.CreateAsyncScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();

        return user;
    }

    private static StringContent CreateJsonContent(string email = "test@gmail.com", string code = PlainCode)
    {
        var payload = new { Email = email, Code = code };

        var json = JsonSerializer.Serialize(payload);

        return new StringContent(json, Encoding.UTF8, "application/json");
    }
}
