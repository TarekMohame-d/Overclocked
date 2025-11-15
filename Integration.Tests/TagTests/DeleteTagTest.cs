using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Api.Routing;
using Application.Common.Results;
using ArchitectureTests.FakeData;
using Domain.Entities;
using Domain.StaticData;
using Infrastructure.Data;
using Integration.Tests.Shared;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Integration.Tests.TagTests;

[Collection(nameof(SharedTestCollection))]
public class DeleteTagTest(CustomWebApplicationFactory factory) : IAsyncLifetime
{
    private readonly HttpClient _client = factory.HttpClient;

    public async Task InitializeAsync()
    {
        await factory.ResetDatabaseAsync();

        var token = CustomWebApplicationFactory
            .GenerateJwtToken(permissions: [PermissionType.AddEditDelete.ToString()]);
        factory.HttpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Delete_Should_ReturnFailure_When_IdNotValid()
    {
        // Arrange
        var tagId = Guid.CreateVersion7().ToString();

        // Act
        HttpResponseMessage response = await _client.DeleteAsync(TagRoutes.Delete.Replace("{id:guid}", tagId));

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);

        Result? result = await response.Content.ReadFromJsonAsync<Result>();

        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBeNull();
        result.Error.Type.ShouldBe(ErrorType.NotFound);
        result.Error.Description.ShouldBe("Tag not found.");
    }

    [Fact]
    public async Task Delete_Should_ReturnSuccess_When_IdIsValid()
    {
        // Arrange
        Tag tag = await SeedDatabaseAsync();

        // Act
        HttpResponseMessage response =
            await _client.DeleteAsync(TagRoutes.Delete.Replace("{id:guid}", tag.Id.ToString()));

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        Result? result = await response.Content.ReadFromJsonAsync<Result>();

        using IServiceScope scope = factory.Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        Tag? deletedTag = await dbContext.Tags.FindAsync(tag.Id);

        deletedTag.ShouldBeNull();

        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBeNull();
    }

    [Fact]
    public async Task Delete_Should_ReturnForbidden_When_UserDoesNotHavePermission()
    {
        // Arrange
        Tag tag = await SeedDatabaseAsync();

        var token = CustomWebApplicationFactory.GenerateJwtToken();
        factory.HttpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        // Act
        HttpResponseMessage response =
            await _client.DeleteAsync(TagRoutes.Delete.Replace("{id:guid}", tag.Id.ToString()));

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    private async Task<Tag> SeedDatabaseAsync()
    {
        Tag tag = new TagFaker().Generate();

        using IServiceScope scope = factory.Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        dbContext.Tags.Add(tag);
        await dbContext.SaveChangesAsync();

        return tag;
    }
}
