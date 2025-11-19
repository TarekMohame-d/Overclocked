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
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Shouldly;

namespace Integration.Tests.AuthenticationTests;

[Collection(nameof(SharedTestCollection))]
public class LoginTest(CustomWebApplicationFactory factory) : IAsyncLifetime
{
    private readonly HttpClient _client = factory.HttpClient;

    public async Task InitializeAsync()
    {
        await factory.ResetDatabaseAsync();
        factory.BackgroundJobClientMock.ClearReceivedCalls();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Login_Should_ReturnSuccess_When_DataIsValid()
    {
        // Arrange
        await SeedDatabaseAsync();
        StringContent form = CreateJsonContent();

        factory.BackgroundJobClientMock.Create(Arg.Any<Job>(), Arg.Any<IState>()).Returns("a-fake-job-id");

        // Act
        HttpResponseMessage response = await _client.PostAsync(AuthRoutes.Login, form);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        Result? result = await response.Content.ReadFromJsonAsync<Result<AuthResponse>>();

        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBeNull();

        factory.BackgroundJobClientMock.DidNotReceive().Create(Arg.Any<Job>(), Arg.Any<EnqueuedState>());
    }

    private async Task<User> SeedDatabaseAsync()
    {
        var passwordHasher = new PasswordHasher();
        User user = new UserFaker().Generate();
        user.Email = "test@gmail.com";
        user.EmailConfirmed = true;
        user.PasswordHash = passwordHasher.Hash("P@ssword123");

        var role = new Role { Name = "Customer", Id = 4 };

        using IServiceScope scope = factory.Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        dbContext.Users.Add(user);
        dbContext.Roles.Add(role);
        await dbContext.SaveChangesAsync();

        return user;
    }

    private static StringContent CreateJsonContent(string email = "test@gmail.com")
    {
        var payload = new
        {
            Email = email,
            Password = "P@ssword123",
            DeviceId = "device-id",
        };

        var json = JsonSerializer.Serialize(payload);

        return new StringContent(json, Encoding.UTF8, "application/json");
    }
}
