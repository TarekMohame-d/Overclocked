using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Api.Routing;
using Application.Common.Results;
using Domain.Entities;
using Hangfire.Common;
using Hangfire.States;
using Infrastructure.Data;
using Integration.Tests.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Shouldly;

namespace Integration.Tests.AuthenticationTests;

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
        await SeedDatabaseAsync();
        const string Email = "test@gmail.com";
        StringContent form = CreateJsonContent();

        factory.BackgroundJobClientMock
            .Create(Arg.Any<Job>(), Arg.Any<IState>())
            .Returns("a-fake-job-id");

        // Act
        HttpResponseMessage response = await _client.PostAsync(AuthRoutes.Register, form);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Created);

        Result? result = await response.Content.ReadFromJsonAsync<Result>();

        using IServiceScope scope = factory.Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        User? userDb = await dbContext.Users.SingleOrDefaultAsync(x => x.Email == Email);
        userDb.ShouldNotBeNull();
        userDb.EmailConfirmed.ShouldBeFalse();

        EmailConfirmationCode? emailConfirmationCodeDb =
            await dbContext.EmailConfirmationCodes.SingleOrDefaultAsync(x => x.UserId == userDb.Id);
        emailConfirmationCodeDb.ShouldNotBeNull();
        emailConfirmationCodeDb.IsUsed.ShouldBeFalse();

        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.Created);
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBeNull();

        factory.BackgroundJobClientMock.Received(1)
            .Create(Arg.Any<Job>(), Arg.Any<EnqueuedState>());
    }

    private async Task SeedDatabaseAsync()
    {
        var role = new Role
        {
            Name = "Customer",
            Id = 4
        };

        using IServiceScope scope = factory.Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        dbContext.Roles.Add(role);
        await dbContext.SaveChangesAsync();
    }

    private static StringContent CreateJsonContent(string email = "test@gmail.com")
    {
        var payload = new
        {
            Email = email,
            Password = "P@ssword123",
            FirstName = "Test",
            LastName = "Test",
            PhoneNumber = "0123456789"
        };

        var json = JsonSerializer.Serialize(payload);

        return new StringContent(json, Encoding.UTF8, "application/json");
    }
}
