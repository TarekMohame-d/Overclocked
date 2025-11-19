using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Api.Routing;
using Application.Common.Results;
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
public class ResendEmailConfirmationCodeTest(CustomWebApplicationFactory factory) : IAsyncLifetime
{
    private const string PlainCode = "VC4R53";
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
        (User user, EmailConfirmationCode emailConfirmationCode) = await SeedDatabaseAsync();
        StringContent form = CreateJsonContent();

        factory.BackgroundJobClientMock.Create(Arg.Any<Job>(), Arg.Any<IState>()).Returns("a-fake-job-id");

        // Act
        HttpResponseMessage response = await _client.PostAsync(AuthRoutes.ResendConfirmationCode, form);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        Result? result = await response.Content.ReadFromJsonAsync<Result>();

        using IServiceScope scope = factory.Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        User? userDb = await dbContext.Users.SingleOrDefaultAsync(x => x.Email == user.Email);
        userDb.ShouldNotBeNull();
        userDb.EmailConfirmed.ShouldBeFalse();

        EmailConfirmationCode? emailConfirmationCodeDb = await dbContext.EmailConfirmationCodes.SingleOrDefaultAsync(
            x => x.UserId == userDb.Id
        );
        emailConfirmationCodeDb.ShouldNotBeNull();
        emailConfirmationCodeDb.IsUsed.ShouldBeFalse();

        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBeNull();

        factory.BackgroundJobClientMock.Received(1).Create(Arg.Any<Job>(), Arg.Any<EnqueuedState>());
    }

    private async Task<(User user, EmailConfirmationCode emailConfirmationCode)> SeedDatabaseAsync()
    {
        var emailConfirmationCodeHasher = new EmailConfirmationCodeHasher();
        User user = new UserFaker().Generate();
        user.Email = "test@gmail.com";
        user.EmailConfirmed = false;
        EmailConfirmationCode emailConfirmationCode = new EmailConfirmationCodeFaker().Generate();
        emailConfirmationCode.IsUsed = false;
        emailConfirmationCode.ExpiredAt = DateTime.UtcNow.AddMinutes(10);
        emailConfirmationCode.CodeHash = emailConfirmationCodeHasher.Hash(PlainCode);
        emailConfirmationCode.UserId = user.Id;

        var role = new Role { Name = "Customer", Id = 4 };

        using IServiceScope scope = factory.Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        dbContext.Users.Add(user);
        dbContext.EmailConfirmationCodes.Add(emailConfirmationCode);
        dbContext.Roles.Add(role);
        await dbContext.SaveChangesAsync();

        return (user, emailConfirmationCode);
    }

    private static StringContent CreateJsonContent(string email = "test@gmail.com")
    {
        var payload = new { Email = email };

        var json = JsonSerializer.Serialize(payload);

        return new StringContent(json, Encoding.UTF8, "application/json");
    }
}
