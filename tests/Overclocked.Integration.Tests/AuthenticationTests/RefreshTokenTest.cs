using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Overclocked.Api.Routing;
using Overclocked.Application.Features.AuthenticationUseCases.DTOs.Responses;
using Overclocked.Architecture.Tests.FakeData;
using Overclocked.Domain.UserAggregate;
using Overclocked.Infrastructure.Authentication;
using Overclocked.Infrastructure.Persistence;
using Overclocked.Integration.Tests.Shared;
using Shouldly;

namespace Overclocked.Integration.Tests.AuthenticationTests;

public class RefreshTokenTest(ApiTestFixture fixture) : IAsyncLifetime
{
    private readonly HttpClient _client = fixture.HttpClient;

    public async ValueTask InitializeAsync() => await fixture.ResetDatabaseAsync();

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    [Fact]
    public async Task RefreshToken_Should_ReturnSuccess_When_DataIsValid()
    {
        // Arrange
        User user = await SeedDatabaseAsync();
        StringContent loginForm = CreateLoginJsonContent(user.Email);

        HttpResponseMessage loginResponse = await _client.PostAsync(AuthRoutes.Login, loginForm);

        AuthResponse? loginResult = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();
        loginResult.ShouldNotBeNull();

        StringContent form = CreateJsonContent(loginResult.AccessToken, loginResult.RefreshToken);
        // Act
        HttpResponseMessage response = await _client.PostAsync(AuthRoutes.RefreshToken, form);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        AuthResponse? result = await response.Content.ReadFromJsonAsync<AuthResponse>();

        result.ShouldNotBeNull();
        result.AccessToken.ShouldNotBe(loginResult.AccessToken);
        result.RefreshToken.ShouldNotBe(loginResult.RefreshToken);

        using IServiceScope scope = fixture.Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        User? userId = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == user.Id);
        userId.ShouldNotBeNull();
    }

    private async Task<User> SeedDatabaseAsync()
    {
        User user = new UserFaker(new PasswordHasher()).Generate();
        user.ConfirmEmail();

        using IServiceScope scope = fixture.Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();

        return user;
    }

    private static StringContent CreateJsonContent(string accessToken, string refreshToken)
    {
        var payload = new { AccessToken = accessToken, RefreshToken = refreshToken };

        var json = JsonSerializer.Serialize(payload);

        return new StringContent(json, Encoding.UTF8, "application/json");
    }

    private static StringContent CreateLoginJsonContent(string email)
    {
        var payload = new
        {
            Email = email,
            Password = "P@ssword123",
            DeviceId = Guid.NewGuid().ToString(),
        };

        var json = JsonSerializer.Serialize(payload);

        return new StringContent(json, Encoding.UTF8, "application/json");
    }
}
