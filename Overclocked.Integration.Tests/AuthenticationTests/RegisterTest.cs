using System.Net;
using System.Text;
using System.Text.Json;
using Hangfire.Common;
using Hangfire.States;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Overclocked.Api.Routing;
using Overclocked.Domain.UserAggregate;
using Overclocked.Infrastructure.Persistence;
using Overclocked.Integration.Tests.Shared;
using Shouldly;

namespace Overclocked.Integration.Tests.AuthenticationTests;

[Collection(nameof(SharedTestCollection))]
public class RegisterTest(CustomWebApplicationFactory factory) : IAsyncLifetime
{
    private readonly HttpClient _client = factory.HttpClient;

    public async Task InitializeAsync()
    {
        await factory.ResetDatabaseAsync();
        factory.BackgroundJobClientMock.ClearReceivedCalls();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Register_Should_ReturnSuccess_When_DataIsValid()
    {
        // Arrange
        const string Email = "test@gmail.com";
        StringContent form = CreateJsonContent(Email);

        // Act
        HttpResponseMessage response = await _client.PostAsync(AuthRoutes.Register, form);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Created);

        using IServiceScope scope = factory.Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        User? userDb = await dbContext.Users.SingleOrDefaultAsync(x => x.Email == Email);
        userDb.ShouldNotBeNull();
        userDb.EmailConfirmed.ShouldBeFalse();
    }

    private static StringContent CreateJsonContent(string email)
    {
        var payload = new
        {
            Email = email,
            Password = "P@ssword123",
            FirstName = "Test",
            LastName = "Test",
            PhoneNumber = "0123456789",
        };

        var json = JsonSerializer.Serialize(payload);

        return new StringContent(json, Encoding.UTF8, "application/json");
    }
}
