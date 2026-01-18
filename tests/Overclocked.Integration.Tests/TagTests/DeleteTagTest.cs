using System.Net;
using System.Net.Http.Headers;
using Microsoft.Extensions.DependencyInjection;
using Overclocked.Api.Routing;
using Overclocked.Architecture.Tests.FakeData;
using Overclocked.Domain.TagAggregate;
using Overclocked.Domain.UserAggregate.Enums;
using Overclocked.Infrastructure.Persistence;
using Overclocked.Integration.Tests.Shared;
using Shouldly;

namespace Overclocked.Integration.Tests.TagTests;

[Collection(nameof(IntegrationTestCollection))]
public class DeleteTagTest(IntegrationTestWebAppFactory factory) : IAsyncLifetime
{
    private readonly HttpClient _client = factory.HttpClient;

    public async Task InitializeAsync()
    {
        await factory.ResetDatabaseAsync();

        var token = factory.GenerateJwtToken(permissions: [nameof(Permission.AddEditDelete)]);
        factory.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
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
    }

    [Fact]
    public async Task Delete_Should_ReturnSuccess_When_IdIsValid()
    {
        // Arrange
        Tag tag = await SeedDatabaseAsync();

        // Act
        HttpResponseMessage response = await _client.DeleteAsync(TagRoutes.Delete.Replace("{id:guid}", tag.Id.Value.ToString()));

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);

        using IServiceScope scope = factory.Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        Tag? deletedTag = await dbContext.Tags.FindAsync(tag.Id);

        deletedTag.ShouldBeNull();
    }

    [Fact]
    public async Task Delete_Should_ReturnForbidden_When_UserDoesNotHavePermission()
    {
        // Arrange
        Tag tag = await SeedDatabaseAsync();

        var token = factory.GenerateJwtToken();
        factory.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        HttpResponseMessage response = await _client.DeleteAsync(TagRoutes.Delete.Replace("{id:guid}", tag.Id.Value.ToString()));

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
