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
using NSubstitute;
using Shouldly;

namespace Integration.Tests.CategoryTests;

[Collection(nameof(SharedTestCollection))]
public class DeleteCategoryTest(CustomWebApplicationFactory factory) : IAsyncLifetime
{
    private readonly HttpClient _client = factory.HttpClient;

    public async Task InitializeAsync()
    {
        await factory.ResetDatabaseAsync();
        factory.FileStorageServiceMock.ClearReceivedCalls();

        var token = CustomWebApplicationFactory
            .GenerateJwtToken(permissions: [PermissionType.AddEditDelete.ToString()]);
        factory.HttpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Delete_Should_ReturnFailure_When_IdNotFound()
    {
        // Arrange
        var categoryId = Guid.CreateVersion7().ToString();

        // Act
        HttpResponseMessage response =
            await _client.DeleteAsync(CategoryRoutes.Delete.Replace("{id:guid}", categoryId));

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);

        Result? result = await response.Content.ReadFromJsonAsync<Result>();

        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBeNull();
        result.Error.Type.ShouldBe(ErrorType.NotFound);
        result.Error.Description.ShouldBe("Category not found.");
    }

    [Fact]
    public async Task Delete_Should_ReturnSuccess_When_IdExists()
    {
        // Arrange
        Category category = await SeedDatabaseAsync();

        // Act
        HttpResponseMessage response =
            await _client.DeleteAsync(CategoryRoutes.Delete.Replace("{id:guid}", category.Id.ToString()));

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        Result? result = await response.Content.ReadFromJsonAsync<Result>();

        using IServiceScope scope = factory.Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        Category? deletedCategory = await dbContext.Categories.FindAsync(category.Id);

        deletedCategory.ShouldBeNull();

        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBeNull();
    }

    [Fact]
    public async Task Delete_Should_ReturnForbidden_When_UserDoesNotHavePermission()
    {
        // Arrange
        Category category = await SeedDatabaseAsync();

        var token = CustomWebApplicationFactory.GenerateJwtToken();
        factory.HttpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        // Act
        HttpResponseMessage response =
            await _client.DeleteAsync(CategoryRoutes.Delete.Replace("{id:guid}", category.Id.ToString()));

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
}
