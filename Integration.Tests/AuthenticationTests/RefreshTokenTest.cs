using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Api.Routing;
using Application.Common.Results;
using Application.Services.Authentication.DTOs.Response;
using ArchitectureTests.FakeData;
using Domain.Entities;
using Hangfire.Common;
using Hangfire.States;
using Infrastructure.Authentication;
using Infrastructure.Data;
using Integration.Tests.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Shouldly;

namespace Integration.Tests.AuthenticationTests;


[Collection(nameof(SharedTestCollection))]
public class RefreshTokenTest(CustomWebApplicationFactory factory) : IAsyncLifetime
{
    private readonly HttpClient _client = factory.HttpClient;

    public async Task InitializeAsync()
    {
        await factory.ResetDatabaseAsync();
        factory.BackgroundJobClientMock.ClearReceivedCalls();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task RefreshToken_Should_ReturnSuccess_When_DataIsValid()
    {
        // Arrange
        User user = await SeedDatabaseAsync();
        StringContent loginForm = CreateLoginJsonContent();

        factory.BackgroundJobClientMock
            .Create(Arg.Any<Job>(), Arg.Any<IState>())
            .Returns("a-fake-job-id");
        HttpResponseMessage loginResponse = await _client.PostAsync(AuthRoutes.Login, loginForm);

        Result<AuthResponse>? loginResult = await loginResponse.Content.ReadFromJsonAsync<Result<AuthResponse>>();
        loginResult.ShouldNotBeNull();
        loginResult.Data.ShouldNotBeNull();
        StringContent form = CreateJsonContent(loginResult.Data.AccessToken, loginResult.Data.RefreshToken);
        // Act
        HttpResponseMessage response = await _client.PostAsync(AuthRoutes.RefreshToken, form);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        Result<AuthResponse>? result = await response.Content.ReadFromJsonAsync<Result<AuthResponse>>();

        result.ShouldNotBeNull();
        result.Data.ShouldNotBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBeNull();
        result.Data.AccessToken.ShouldNotBe(loginResult.Data.AccessToken);
        result.Data.RefreshToken.ShouldNotBe(loginResult.Data.RefreshToken);

        using IServiceScope scope = factory.Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        RefreshToken? refreshTokenDb = await dbContext.RefreshTokens.SingleOrDefaultAsync(x =>
            x.UserId == user.Id && x.DeviceId == "device-id");
        refreshTokenDb.ShouldNotBeNull();
        new RefreshTokenHasher().Verify(result.Data.RefreshToken, refreshTokenDb.TokenHash).ShouldBeTrue();

        factory.BackgroundJobClientMock.DidNotReceive()
            .Create(Arg.Any<Job>(), Arg.Any<EnqueuedState>());
    }

    private async Task<User> SeedDatabaseAsync()
    {
        var passwordHasher = new PasswordHasher();
        User user = new UserFaker().Generate();
        user.Email = "test@gmail.com";
        user.EmailConfirmed = true;
        user.PasswordHash = passwordHasher.Hash("P@ssword123");

        var role = new Role
        {
            Name = "Customer",
            Id = 4
        };

        using IServiceScope scope = factory.Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        dbContext.Users.Add(user);
        dbContext.Roles.Add(role);
        await dbContext.SaveChangesAsync();

        return user;
    }

    private static StringContent CreateJsonContent(string accessToken, string refreshToken)
    {
        var payload = new
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };

        var json = JsonSerializer.Serialize(payload);

        return new StringContent(json, Encoding.UTF8, "application/json");
    }

    private static StringContent CreateLoginJsonContent(string email = "test@gmail.com")
    {
        var payload = new
        {
            Email = email,
            Password = "P@ssword123",
            DeviceId = "device-id"
        };

        var json = JsonSerializer.Serialize(payload);

        return new StringContent(json, Encoding.UTF8, "application/json");
    }
}
