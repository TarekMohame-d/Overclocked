using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Api.Common.Routing;
using Application.Common.Results;
using ArchitectureTests.FakeData;
using Infrastructure.Data;
using Integration.Tests.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Domain.Entities;
using System.Text.Json;
using System.Text;
using NSubstitute;

namespace Integration.Tests.CategoryTests;

[Collection(nameof(SharedTestCollection))]
public class CreateCategoryTest : IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public CreateCategoryTest(CustomWebApplicationFactory factory)
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
    public async Task Create_Should_CreateAndReturnSuccess_When_DataIsValid()
    {
        // Arrange
        var name = "NVIDIA";

        var form = CreateJsonContent(name);

        // Act
        var response = await _client.PostAsync(CategoryRoutes.Create, form);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Created);

        var result = await response.Content.ReadFromJsonAsync<Result>();

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var category = await dbContext.Categories.SingleOrDefaultAsync(x => x.NormalizedName == name.ToUpper());
        category.ShouldNotBeNull();

        category.Name.ShouldBe(name);

        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.Created);
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBeNull();
    }

    [Fact]
    public async Task Create_Should_ReturnBadRequest_When_NameAlreadyExists()
    {
        // Arrange
        var category = await SeedDatabaseAsync();
        var form = CreateJsonContent(category.Name);

        // Act
        var response = await _client.PostAsync(CategoryRoutes.Create, form);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        var result = await response.Content.ReadFromJsonAsync<Result>();

        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBeNull();
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
