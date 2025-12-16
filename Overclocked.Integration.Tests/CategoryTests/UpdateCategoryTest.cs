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

[Collection(nameof(SharedTestCollection))]
public class UpdateCategoryTest(CustomWebApplicationFactory factory) : IAsyncLifetime
{
    private readonly HttpClient _client = factory.HttpClient;

    public async Task InitializeAsync()
    {
        await factory.ResetDatabaseAsync();
        factory.FileStorageServiceMock.ClearReceivedCalls();
        factory.BackgroundJobClientMock.ClearReceivedCalls();

        var token = CustomWebApplicationFactory.GenerateJwtToken(
            permissions: [Permission.AddEditDelete.ToString()]);
        factory.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public Task DisposeAsync() => Task.CompletedTask;

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
        StringContent form = CreateJsonContent("New Name", category.ImageUrl);

        // Act
        HttpResponseMessage response = await _client.PutAsync(
            CategoryRoutes.Update.Replace("{id:guid}", category.Id.Value.ToString()), form);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);

        using IServiceScope scope = factory.Services.CreateScope();
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

        factory.BackgroundJobClientMock.Create(Arg.Any<Job>(), Arg.Any<IState>()).Returns("a-fake-job-id");

        // Act
        HttpResponseMessage response = await _client.PutAsync(
            CategoryRoutes.Update.Replace("{id:guid}", category.Id.Value.ToString()), form);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);

        using IServiceScope scope = factory.Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        Category? updatedCategory = await dbContext.Categories.FindAsync(category.Id);
        updatedCategory.ShouldNotBeNull();
        updatedCategory.Name.ShouldBe("New Name");
        updatedCategory.ImageUrl.ShouldBe("https://res.cloudinary.com/over-clocked/new-image.jpg");
    }

    [Fact]
    public async Task Update_Should_ReturnForbidden_When_UserDoesNotHavePermission()
    {
        // Arrange
        Category category = await SeedDatabaseAsync();
        StringContent form = CreateJsonContent("New Name", category.ImageUrl);

        var token = CustomWebApplicationFactory.GenerateJwtToken();
        factory.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        HttpResponseMessage response = await _client.PutAsync(
            CategoryRoutes.Update.Replace("{id:guid}", category.Id.Value.ToString()), form);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    private async Task<Category> SeedDatabaseAsync()
    {
        Category category = new CategoryFaker().Generate();

        using IServiceScope scope = factory.Services.CreateScope();
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
