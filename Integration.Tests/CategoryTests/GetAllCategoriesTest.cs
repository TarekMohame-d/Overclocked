using System.Net;
using System.Net.Http.Json;
using Api.Routing;
using Application.Abstraction.Services;
using Application.Common.Constants;
using Application.Common.Results;
using Application.Services.Category.DTOs.Response;
using Application.Services.Category.Mapping;
using ArchitectureTests.FakeData;
using Domain.Entities;
using Infrastructure.Data;
using Integration.Tests.Shared;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Integration.Tests.CategoryTests;

[Collection(nameof(SharedTestCollection))]
public class GetAllCategoriesTest(CustomWebApplicationFactory factory) : IAsyncLifetime
{
    private readonly HttpClient _client = factory.HttpClient;

    public async Task InitializeAsync() => await factory.ResetDatabaseAsync();

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task GetAll_Should_ReturnFromDatabase_When_ThereIsDataAndCacheMiss()
    {
        // Arrange
        IEnumerable<Category> categories = await SeedDatabaseAsync();

        // Act
        HttpResponseMessage response = await _client.GetAsync(CategoryRoutes.GetAll);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        Result<IEnumerable<CategoryListResponse>>? result = await response.Content.ReadFromJsonAsync<
            Result<IEnumerable<CategoryListResponse>>
        >();

        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.Data.ShouldNotBeNull();
        result.Data.ShouldNotBeEmpty();
        result.Data.Count().ShouldBe(categories.Count());
    }

    [Fact]
    public async Task GetAll_Should_ReturnFromCache_When_ThereIsDataAndCacheHit()
    {
        // Arrange
        IEnumerable<CategoryListResponse> categoryListDtos = await SeedCacheAsync();

        // Act
        HttpResponseMessage response = await _client.GetAsync(CategoryRoutes.GetAll);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        Result<IEnumerable<CategoryListResponse>>? result = await response.Content.ReadFromJsonAsync<
            Result<IEnumerable<CategoryListResponse>>
        >();

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

        // Act
        HttpResponseMessage response = await _client.GetAsync(CategoryRoutes.GetAll);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        Result<IEnumerable<CategoryListResponse>>? result = await response.Content.ReadFromJsonAsync<
            Result<IEnumerable<CategoryListResponse>>
        >();

        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.Data.ShouldNotBeNull();
        result.Data.ShouldBeEmpty();
        result.Data.Count().ShouldBe(0);
    }

    [Fact]
    public async Task GetAll_Should_ReturnConsistentResults_When_CalledConcurrently()
    {
        // Arrange
        IEnumerable<Category> categories = await SeedDatabaseAsync();

        const int ConcurrentCalls = 10;
        var tasks = new List<Task<HttpResponseMessage>>();

        // Act
        for(var i = 0; i < ConcurrentCalls; i++)
            tasks.Add(_client.GetAsync(CategoryRoutes.GetAll));

        await Task.WhenAll(tasks);

        // Assert
        foreach(Task<HttpResponseMessage> task in tasks)
        {
            HttpResponseMessage response = await task;
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            Result<IEnumerable<CategoryListResponse>>? result = await response.Content.ReadFromJsonAsync<
                Result<IEnumerable<CategoryListResponse>>
            >();

            result.ShouldNotBeNull();
            result.IsSuccess.ShouldBeTrue();
            result.StatusCode.ShouldBe(HttpStatusCode.OK);
            result.Data.ShouldNotBeNull();
            result.Data.ShouldNotBeEmpty();
            result.Data.Count().ShouldBe(categories.Count());
        }
    }

    private async Task<IEnumerable<Category>> SeedDatabaseAsync()
    {
        using IServiceScope scope = factory.Services.CreateScope();

        List<Category> categories = new CategoryFaker().Generate(10);
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await dbContext.Categories.AddRangeAsync(categories);
        await dbContext.SaveChangesAsync();

        return categories;
    }

    private async Task<IEnumerable<CategoryListResponse>> SeedCacheAsync()
    {
        using IServiceScope scope = factory.Services.CreateScope();

        List<Category> categories = new CategoryFaker().Generate(10);

        ICacheService cache = scope.ServiceProvider.GetRequiredService<ICacheService>();
        var key = CacheKeys.AllCategories;
        IEnumerable<CategoryListResponse> categoryListDtos = categories.ToDto();
        var result = Result<IEnumerable<CategoryListResponse>>.Success(categoryListDtos);
        await cache.SetAsync(key, result, TimeSpan.FromMinutes(5));

        return categoryListDtos;
    }
}
