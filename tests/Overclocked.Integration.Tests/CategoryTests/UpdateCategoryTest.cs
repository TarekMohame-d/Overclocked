using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Hangfire.Common;
using Hangfire.States;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Overclocked.Api.Routing;
using Overclocked.Architecture.Tests.FakeData;
using Overclocked.Domain.CategoryAggregate;
using Overclocked.Domain.UserAggregate.Enums;
using Overclocked.Infrastructure.Persistence;
using Overclocked.Integration.Tests.Shared;
using Shouldly;

namespace Overclocked.Integration.Tests.CategoryTests;

public class UpdateCategoryTest(IntegrationTestWebAppFactory fixture) : IAsyncLifetime
{
    private readonly HttpClient _client = fixture.HttpClient;

    public async ValueTask InitializeAsync()
    {
        await fixture.ResetDatabaseAsync();
        fixture.FileStorageServiceMock.ClearReceivedCalls();
        fixture.BackgroundJobClientMock.ClearReceivedCalls();

        var token = fixture.GenerateJwtToken(permissions: [nameof(Permission.AddEditDelete)]);
        fixture.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    [Fact]
    public async Task Update_Should_ReturnFailure_When_IdNotFound()
    {
        // Arrange
        const string Name = "NVIDIA";
        var id = Guid.NewGuid().ToString();
        StringContent form = CreateJsonContent(Name, "https://res.cloudinary.com/over-clocked/image.jpg");

        // Act
        HttpResponseMessage response = await _client.PutAsync(CategoryRoutes.Update.Replace("{id:guid}", id), form);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Update_Should_ReturnSuccess_When_CategoryExistsAndOldImageUrlIsProvided()
    {
        // Arrange
        Category category = await SeedDatabaseAsync();
        StringContent form = CreateJsonContent("New Name", category.Image.Value);

        // Act
        HttpResponseMessage response = await _client.PutAsync(
            CategoryRoutes.Update.Replace("{id:guid}", category.Id.Value.ToString()),
            form
        );

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);

        using IServiceScope scope = fixture.Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        Category? updatedCategory = await dbContext.Categories.FindAsync(category.Id);
        updatedCategory.ShouldNotBeNull();
        updatedCategory.Name.ShouldBe("New Name");
    }

    [Fact]
    public async Task Update_Should_ReturnSuccess_When_CategoryExistsAndNewImageUrlIsProvided()
    {
        // Arrange
        Category category = await SeedDatabaseAsync();
        StringContent form = CreateJsonContent("New Name", "https://res.cloudinary.com/over-clocked/new-image.jpg");

        fixture.BackgroundJobClientMock.Create(Arg.Any<Job>(), Arg.Any<IState>()).Returns("a-fake-job-id");

        // Act
        HttpResponseMessage response = await _client.PutAsync(
            CategoryRoutes.Update.Replace("{id:guid}", category.Id.Value.ToString()),
            form
        );

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);

        using IServiceScope scope = fixture.Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        Category? updatedCategory = await dbContext.Categories.FindAsync(category.Id);
        updatedCategory.ShouldNotBeNull();
        updatedCategory.Name.ShouldBe("New Name");
        updatedCategory.Image.Value.ShouldBe("https://res.cloudinary.com/over-clocked/new-image.jpg");
    }

    [Fact]
    public async Task Update_Should_ReturnForbidden_When_UserDoesNotHavePermission()
    {
        // Arrange
        Category category = await SeedDatabaseAsync();
        StringContent form = CreateJsonContent("New Name", category.Image.Value);

        var token = fixture.GenerateJwtToken();
        fixture.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        HttpResponseMessage response = await _client.PutAsync(
            CategoryRoutes.Update.Replace("{id:guid}", category.Id.Value.ToString()),
            form
        );

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    private async Task<Category> SeedDatabaseAsync()
    {
        Category category = new CategoryFaker().Generate();

        using IServiceScope scope = fixture.Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        dbContext.Categories.Add(category);
        await dbContext.SaveChangesAsync();

        return category;
    }

    private static StringContent CreateJsonContent(string name, string imageUrl)
    {
        var payload = new { Name = name, ImageUrl = imageUrl };

        var json = JsonSerializer.Serialize(payload);

        return new StringContent(json, Encoding.UTF8, "application/json");
    }
}
