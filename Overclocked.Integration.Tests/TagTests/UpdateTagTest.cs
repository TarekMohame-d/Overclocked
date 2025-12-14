using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Overclocked.Api.Routing;
using Overclocked.Architecture.Tests.FakeData;
using Overclocked.Domain.Common.StaticData;
using Overclocked.Domain.TagAggregate;
using Overclocked.Infrastructure.Persistence;
using Overclocked.Integration.Tests.Shared;
using Shouldly;

namespace Overclocked.Integration.Tests.TagTests;

[Collection(nameof(SharedTestCollection))]
public class UpdateTagTest(CustomWebApplicationFactory factory) : IAsyncLifetime
{
    private readonly HttpClient _client = factory.HttpClient;

    public async Task InitializeAsync()
    {
        await factory.ResetDatabaseAsync();

        var token = CustomWebApplicationFactory.GenerateJwtToken(
            permissions: [PermissionType.AddEditDelete.ToString()]);
        factory.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Update_Should_ReturnFailure_When_IdNotValid()
    {
        // Arrange
        const string Name = "AMD";
        var id = Guid.NewGuid().ToString();
        StringContent form = CreateJsonContent(Name);

        // Act
        HttpResponseMessage response = await _client.PutAsync(TagRoutes.Update.Replace("{id:guid}", id), form);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Update_Should_ReturnSuccess_When_DataIsValid()
    {
        // Arrange
        Tag tag = await SeedDatabaseAsync();
        StringContent form = CreateJsonContent("New Name");

        // Act
        HttpResponseMessage response = await _client.PutAsync(
            TagRoutes.Update.Replace("{id:guid}", tag.Id.Value.ToString()), form);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);

        using IServiceScope scope = factory.Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        Tag? updatedTag = await dbContext.Tags.FindAsync(tag.Id);
        updatedTag.ShouldNotBeNull();
        updatedTag.Name.ShouldBe("New Name");
    }

    [Fact]
    public async Task Update_Should_ReturnForbidden_When_UserDoesNotHavePermission()
    {
        // Arrange
        Tag tag = await SeedDatabaseAsync();
        StringContent form = CreateJsonContent("New Name");

        var token = CustomWebApplicationFactory.GenerateJwtToken();
        factory.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        HttpResponseMessage response = await _client.PutAsync(
            TagRoutes.Update.Replace("{id:guid}", tag.Id.Value.ToString()), form);

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

    private static StringContent CreateJsonContent(string name)
    {
        var payload = new { Name = name };

        var json = JsonSerializer.Serialize(payload);

        return new StringContent(json, Encoding.UTF8, "application/json");
    }
}
