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

namespace Integration.Tests.CategoryTests.Commands;

[Collection(nameof(SharedTestCollection))]
public class DeleteCategoryTest : IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public DeleteCategoryTest(CustomWebApplicationFactory factory)
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
    public async Task Delete_WhenIdNotValid_ShouldReturnFailure()
    {
        // Arrange
        var categoryId = Guid.NewGuid().ToString();

        // Act
        var response = await _client.DeleteAsync(CategoryRoutes.Delete.Replace("{id:guid}", categoryId));

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
    public async Task Delete_WhenIdIsValid_ShouldReturnSuccess()
    {
        // Arrange
        var category = await SeedDatabaseAsync();

        // Act
        var response = await _client.DeleteAsync(CategoryRoutes.Delete.Replace("{id:guid}", category.Id.ToString()));

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<Result>();

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        Category? deletedCategory = await dbContext.Categories.FindAsync(category.Id);

        deletedCategory.ShouldBeNull();

        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBeNull();
    }

    private async Task<Category> SeedDatabaseAsync()
    {
        var category = new CategoryFaker().Generate();

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        dbContext.Categories.Add(category);
        await dbContext.SaveChangesAsync();

        return category;
    }
}
