using Api.Common.Routing;
using Application.Abstraction.Services;
using Application.Common.Constants;
using Application.Common.Results;
using Application.Features.Category.Mapping;
using Application.Features.Category.Queries.GetAllCategories;
using ArchitectureTests.FakeData;
using Infrastructure.Data;
using Integration.Tests.Shared;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Net;
using System.Net.Http.Json;
using CategoryEntity = Domain.Entities.Category;

namespace Integration.Tests.CategoryTests.Queries;

[Collection(nameof(SharedTestCollection))]
public class GetAllCategoryTest : IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public GetAllCategoryTest(CustomWebApplicationFactory factory)
    {
        _client = factory.HttpClient;
        _factory = factory;
    }

    public async Task InitializeAsync() => await _factory.ResetDatabaseAsync();
    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task GetAll_WhenThereIsDataAndCacheMiss_ShouldReturnAllFromDatabase()
    {
        // Arrange
        var categories = await SeedDatabaseAsync();

        // Act
        var response = await _client.GetAsync(CategoryRoutes.GetAll);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<Result<IEnumerable<CategoryListDto>>>();

        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.Data.ShouldNotBeNull();
        result.Data.ShouldNotBeEmpty();
        result.Data.Count().ShouldBe(categories.Count());
    }

    [Fact]
    public async Task GetAll_WhenThereIsDataAndCacheHit_ShouldReturnAllFromCache()
    {
        // Arrange
        var categoryListDtos = await SeedCacheAsync();

        // Act
        var response = await _client.GetAsync(CategoryRoutes.GetAll);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<Result<IEnumerable<CategoryListDto>>>();

        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.Data.ShouldNotBeNull();
        result.Data.ShouldNotBeEmpty();
        result.Data.Count().ShouldBe(categoryListDtos.Count());
    }

    [Fact]
    public async Task GetAll_WhenThereIsNoData_ShouldReturnEmptyList()
    {
        // Arrange
        IEnumerable<CategoryEntity> categories = [];

        // Act
        var response = await _client.GetAsync(CategoryRoutes.GetAll);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<Result<IEnumerable<CategoryListDto>>>();

        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.Data.ShouldNotBeNull();
        result.Data.ShouldBeEmpty();
        result.Data.Count().ShouldBe(categories.Count());
    }

    [Fact]
    public async Task GetAll_WhenCalledConcurrently_ShouldReturnConsistentResults()
    {
        // Arrange
        var categories = await SeedDatabaseAsync();

        int concurrentCalls = 10;
        var tasks = new List<Task<HttpResponseMessage>>();

        // Act
        for (int i = 0; i < concurrentCalls; i++)
        {
            tasks.Add(_client.GetAsync(CategoryRoutes.GetAll));
        }

        await Task.WhenAll(tasks);

        // Assert
        foreach (var task in tasks)
        {
            var response = await task;
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<Result<IEnumerable<CategoryListDto>>>();

            result.ShouldNotBeNull();
            result.IsSuccess.ShouldBeTrue();
            result.StatusCode.ShouldBe(HttpStatusCode.OK);
            result.Data.ShouldNotBeNull();
            result.Data.ShouldNotBeEmpty();
            result.Data.Count().ShouldBe(categories.Count());
        }
    }

    private async Task<IEnumerable<CategoryEntity>> SeedDatabaseAsync()
    {
        using var scope = _factory.Services.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var categories = new CategoryFaker().Generate(10);

        await dbContext.Categories.AddRangeAsync(categories);
        await dbContext.SaveChangesAsync();

        return categories;
    }

    private async Task<IEnumerable<CategoryListDto>> SeedCacheAsync()
    {
        using var scope = _factory.Services.CreateScope();

        var categories = new CategoryFaker().Generate(10);

        var cache = scope.ServiceProvider.GetRequiredService<ICacheService>();
        var key = CacheKeys.AllCategories;
        var categoryListDtos = categories.ToDto();
        var result = Result<IEnumerable<CategoryListDto>>.Success(categoryListDtos);
        await cache.SetAsync(key, result, TimeSpan.FromMinutes(5));

        return categoryListDtos;
    }
}
