using Api.Common.Routing;
using ArchitectureTests.FakeData;
using Infrastructure.Data;
using Integration.Tests.Shared;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Domain.Entities;
using Application.Common.Results;

namespace Integration.Tests.TagTests;

[Collection(nameof(SharedTestCollection))]
public class DeleteTagTest : IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public DeleteTagTest(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.HttpClient;
    }

    public async Task InitializeAsync()
    {
        await _factory.ResetDatabaseAsync();

        var token = _factory.GenerateJwtToken();
        _factory.HttpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
    }
    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Delete_Should_ReturnFailure_When_IdNotValid()
    {
        // Arrange
        var tagId = Guid.NewGuid().ToString();

        // Act
        var response = await _client.DeleteAsync(TagRoutes.Delete.Replace("{id:guid}", tagId));

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);

        var result = await response.Content.ReadFromJsonAsync<Result>();

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
        var tag = await SeedDatabaseAsync();

        // Act
        var response = await _client.DeleteAsync(TagRoutes.Delete.Replace("{id:guid}", tag.Id.ToString()));

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<Result>();

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        Tag? deletedTag = await dbContext.Tags.FindAsync(tag.Id);

        deletedTag.ShouldBeNull();

        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBeNull();
    }

    private async Task<Tag> SeedDatabaseAsync()
    {
        var tag = new TagFaker().Generate();

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        dbContext.Tags.Add(tag);
        await dbContext.SaveChangesAsync();

        return tag;
    }
}
