using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
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

[Collection(nameof(IntegrationTestCollection))]
public class CreateCategoryTest(IntegrationTestWebAppFactory factory) : IAsyncLifetime
{
    private readonly HttpClient _client = factory.HttpClient;

    public async Task InitializeAsync()
    {
        await factory.ResetDatabaseAsync();
        factory.FileStorageServiceMock.ClearReceivedCalls();

        var token = factory.GenerateJwtToken(permissions: [nameof(Permission.AddEditDelete)]);
        factory.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Create_Should_CreateAndReturnSuccess_When_DataIsValid()
    {
        // Arrange
        const string Name = "NVIDIA";

        StringContent form = CreateJsonContent(Name);

        // Act
        HttpResponseMessage response = await _client.PostAsync(CategoryRoutes.Create, form);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Created);

        using IServiceScope scope = factory.Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        Category? category = await dbContext.Categories.SingleOrDefaultAsync(x => x.NormalizedName == Name.ToUpper());
        category.ShouldNotBeNull();

        category.Name.ShouldBe(Name);
    }

    [Fact]
    public async Task Create_Should_ReturnBadRequest_When_NameAlreadyExists()
    {
        // Arrange
        Category category = await SeedDatabaseAsync();
        StringContent form = CreateJsonContent(category.Name);

        // Act
        HttpResponseMessage response = await _client.PostAsync(CategoryRoutes.Create, form);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Create_Should_ReturnForbidden_When_UserDoesNotHavePermission()
    {
        // Arrange
        Category category = await SeedDatabaseAsync();
        StringContent form = CreateJsonContent(category.Name);

        var token = factory.GenerateJwtToken();
        factory.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        HttpResponseMessage response = await _client.PostAsync(CategoryRoutes.Create, form);

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

    private static StringContent CreateJsonContent(
        string name,
        string imageUrl = "https://res.cloudinary.com/over-clocked/image.jpg"
    )
    {
        var payload = new { Name = name, ImageUrl = imageUrl };

        var json = JsonSerializer.Serialize(payload);

        return new StringContent(json, Encoding.UTF8, "application/json");
    }
}
