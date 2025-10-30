using Api.Common.Routing;
using Infrastructure.Data;
using Integration.Tests.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Domain.Entities;
using Application.Common.Results;
using ArchitectureTests.FakeData;
using System.Text.Json;
using System.Text;
using NSubstitute;

namespace Integration.Tests.CategoryTests;

[Collection(nameof(SharedTestCollection))]
public class UpdateCategoryTest : IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public UpdateCategoryTest(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.HttpClient;
    }

    public async Task InitializeAsync()
    {
        await _factory.ResetDatabaseAsync();
        _factory.FileStorageServiceMock.ClearReceivedCalls();

        var token = _factory.GenerateJwtToken();
        _factory.HttpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
    }
    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Update_Should_ReturnFailure_When_IdNotFound()
    {
        // Arrange
        var name = "NVIDIA";
        var id = Guid.NewGuid().ToString();
        var form = CreateJsonContent(name);

        // Act
        var response = await _client.PutAsync(CategoryRoutes.Update.Replace("{id:guid}", id), form);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);

        var result = await response.Content.ReadFromJsonAsync<Result>();

        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBeNull();
        result.Error.Type.ShouldBe(ErrorType.NotFound);
        result.Error.Description.ShouldBe("Category not found.");
    }

    [Fact]
    public async Task Update_Should_ReturnSuccess_When_CategoryExistsAndOldImageUrlIsProvided()
    {
        // Arrange
        var category = await SeedDatabaseAsync();
        var form = CreateJsonContent("New Name", category.Image);

        // Act
        var response = await _client.PutAsync(CategoryRoutes.Update.Replace("{id:guid}", category.Id.ToString()), form);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<Result>();

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        Category? updatedCategory = await dbContext.Categories.FindAsync(category.Id);
        updatedCategory.ShouldNotBeNull();
        updatedCategory.Name.ShouldBe("New Name");

        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBeNull();
    }

    [Fact]
    public async Task Update_Should_ReturnSuccess_When_CategoryExistsAndNewImageUrlIsProvided()
    {
        // Arrange
        var category = await SeedDatabaseAsync();
        var form = CreateJsonContent("New Name", "https://res.cloudinary.com/over-clocked/new-image.jpg");

        // Act
        var response = await _client.PutAsync(CategoryRoutes.Update.Replace("{id:guid}", category.Id.ToString()), form);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<Result>();

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        Category? updatedCategory = await dbContext.Categories.FindAsync(category.Id);
        updatedCategory.ShouldNotBeNull();
        updatedCategory.Name.ShouldBe("New Name");
        updatedCategory.Image.ShouldBe("https://res.cloudinary.com/over-clocked/new-image.jpg");

        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBeNull();

        await _factory.FileStorageServiceMock.Received(1)
            .DeleteFileAsync(
            Arg.Is<string>(url => url == "https://res.cloudinary.com/over-clocked/image.jpg"),
            Arg.Any<CancellationToken>());
    }

    private async Task<Category> SeedDatabaseAsync()
    {
        Category category = new CategoryFaker().Generate();
        category.Image = "https://res.cloudinary.com/over-clocked/image.jpg";

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        dbContext.Categories.Add(category);
        await dbContext.SaveChangesAsync();

        return category;
    }

    private StringContent CreateJsonContent(string name, string imageUrl = "https://res.cloudinary.com/over-clocked/image.jpg")
    {
        var payload = new
        {
            Name = name,
            ImageUrl = imageUrl
        };

        string json = JsonSerializer.Serialize(payload);

        return new StringContent(json, Encoding.UTF8, "application/json");
    }
}
