using Api.Common.Routing;
using Application.Common.Constants;
using Application.Common.Results;
using Application.Features.Category.Mapping;
using ArchitectureTests.FakeData;
using Infrastructure.Data;
using Integration.Tests.Shared;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Net;
using System.Net.Http.Json;
using Domain.Entities;
using Application.Services.Category.DTOs.Response;
using Application.Services;

namespace Integration.Tests.CategoryTests;

[Collection(nameof(SharedTestCollection))]
public class GetAllCategoriesTest : IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public GetAllCategoriesTest(CustomWebApplicationFactory factory)
    {
        _client = factory.HttpClient;
        _factory = factory;
    }

    public async Task InitializeAsync() => await _factory.ResetDatabaseAsync();
    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task GetAll_Should_ReturnFromDatabase_When_ThereIsDataAndCacheMiss()
    {
        // Arrange
        var categorys = await SeedDatabaseAsync();

        // Act
        var response = await _client.GetAsync(CategoryRoutes.GetAll);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<Result<IEnumerable<CategoryListResponse>>>();

        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.Data.ShouldNotBeNull();
        result.Data.ShouldNotBeEmpty();
        result.Data.Count().ShouldBe(categorys.Count());
    }

    [Fact]
    public async Task GetAll_Should_ReturnFromCache_When_ThereIsDataAndCacheHit()
    {
        // Arrange
        var categoryListDtos = await SeedCacheAsync();

        // Act
        var response = await _client.GetAsync(CategoryRoutes.GetAll);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<Result<IEnumerable<CategoryListResponse>>>();

        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.Data.ShouldNotBeNull();
        result.Data.ShouldNotBeEmpty();
        result.Data.Count().ShouldBe(categoryListDtos.Count());
    }

    [Fact]
    public async Task GetAll_Should_ReturnEmptyList_When_ThereIsNoData()
    {
        // Arrange
        IEnumerable<Category> categorys = [];

        // Act
        var response = await _client.GetAsync(CategoryRoutes.GetAll);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<Result<IEnumerable<CategoryListResponse>>>();

        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.Data.ShouldNotBeNull();
        result.Data.ShouldBeEmpty();
        result.Data.Count().ShouldBe(categorys.Count());
    }

    [Fact]
    public async Task GetAll_Should_ReturnConsistentResults_When_CalledConcurrently()
    {
        // Arrange
        var categorys = await SeedDatabaseAsync();

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

            var result = await response.Content.ReadFromJsonAsync<Result<IEnumerable<CategoryListResponse>>>();

            result.ShouldNotBeNull();
            result.IsSuccess.ShouldBeTrue();
            result.StatusCode.ShouldBe(HttpStatusCode.OK);
            result.Data.ShouldNotBeNull();
            result.Data.ShouldNotBeEmpty();
            result.Data.Count().ShouldBe(categorys.Count());
        }
    }

    private async Task<IEnumerable<Category>> SeedDatabaseAsync()
    {
        using var scope = _factory.Services.CreateScope();

        var categorys = new CategoryFaker().Generate(10);
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await dbContext.Categories.AddRangeAsync(categorys);
        await dbContext.SaveChangesAsync();

        return categorys;
    }

    private async Task<IEnumerable<CategoryListResponse>> SeedCacheAsync()
    {
        using var scope = _factory.Services.CreateScope();

        var categorys = new CategoryFaker().Generate(10);

        var cache = scope.ServiceProvider.GetRequiredService<ICacheService>();
        var key = CacheKeys.AllCategories;
        var categoryListDtos = categorys.ToDto();
        var result = Result<IEnumerable<CategoryListResponse>>.Success(categoryListDtos);
        await cache.SetAsync(key, result, TimeSpan.FromMinutes(5));

        return categoryListDtos;
    }
}
