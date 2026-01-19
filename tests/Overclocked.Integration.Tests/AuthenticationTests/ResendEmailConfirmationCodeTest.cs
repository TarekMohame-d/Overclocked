using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Overclocked.Api.Routing;
using Overclocked.Architecture.Tests.FakeData;
using Overclocked.Domain.UserAggregate;
using Overclocked.Infrastructure.Authentication;
using Overclocked.Infrastructure.Persistence;
using Overclocked.Integration.Tests.Shared;
using Shouldly;

namespace Overclocked.Integration.Tests.AuthenticationTests;

[Collection(nameof(IntegrationTestCollection))]
public class ResendEmailConfirmationCodeTest(IntegrationTestWebAppFactory factory) : IAsyncLifetime
{
    private readonly HttpClient _client = factory.HttpClient;

    public async Task InitializeAsync() => await factory.ResetDatabaseAsync();

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task ResendEmailConfirmationCode_Should_ReturnSuccess_When_DataIsValid()
    {
        // Arrange
        User user = await SeedDatabaseAsync();
        StringContent form = CreateJsonContent(user.Email);

        // Act
        HttpResponseMessage response = await _client.PostAsync(AuthRoutes.ResendConfirmationCode, form);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        using IServiceScope scope = factory.Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        User? userDb = await dbContext
            .Users.Include(u => u.EmailConfirmationCode)
            .FirstOrDefaultAsync(x => x.Email == user.Email);

        userDb.ShouldNotBeNull();
        userDb.EmailConfirmed.ShouldBeFalse();

        userDb.EmailConfirmationCode.IsUsed.ShouldBeFalse();
    }

    private async Task<User> SeedDatabaseAsync()
    {
        User user = new UserFaker(new PasswordHasher()).Generate();

        using IServiceScope scope = factory.Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();

        return user;
    }

    private static StringContent CreateJsonContent(string email)
    {
        var payload = new { Email = email };

        var json = JsonSerializer.Serialize(payload);

        return new StringContent(json, Encoding.UTF8, "application/json");
    }
}
