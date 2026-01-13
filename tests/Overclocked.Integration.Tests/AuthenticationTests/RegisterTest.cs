using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Overclocked.Api.Routing;
using Overclocked.Domain.UserAggregate;
using Overclocked.Infrastructure.Persistence;
using Overclocked.Integration.Tests.Shared;
using Shouldly;

namespace Overclocked.Integration.Tests.AuthenticationTests;

public class RegisterTest(ApiTestFixture fixture) : IAsyncLifetime
{
    private readonly HttpClient _client = fixture.HttpClient;

    public async ValueTask InitializeAsync() => await fixture.ResetDatabaseAsync();

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

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

        using IServiceScope scope = fixture.Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        User? userDb = await dbContext.Users.FirstOrDefaultAsync(x => x.Email == Email);
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
