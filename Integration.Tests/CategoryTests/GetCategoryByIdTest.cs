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
public class GetCategoryByIdTest : IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public GetCategoryByIdTest(CustomWebApplicationFactory factory)
    {
        _client = factory.HttpClient;
        _factory = factory;
    }

    public async Task InitializeAsync() => await _factory.ResetDatabaseAsync();
    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task GetById_Should_ReturnFailure_When_NotFound()
    {
        // Arrange
        Guid id = Guid.CreateVersion7();

        // Act
        var response = await _client.GetAsync(CategoryRoutes.GetById.Replace("{id:guid}", id.ToString()));

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);

        var result = await response.Content.ReadFromJsonAsync<Result<CategoryResponse>>();

        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeFalse();
        result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        result.Data.ShouldBeNull();
        result.Error.ShouldNotBeNull();
        result.Error.Type.ShouldBe(ErrorType.NotFound);
    }

    [Fact]
    public async Task GetById_Should_ReturnNotFound_When_IdIsMalformedGuid()
    {
        // Arrange
        var wrongId = "abc";

        // Act
        var response = await _client.GetAsync(CategoryRoutes.GetById.Replace("{id:guid}", wrongId.ToString()));

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetById_Should_ReturnCategoryFromDatabase_When_CacheMiss()
    {
        // Arrange
        var category = await SeedDatabaseAsync();

        // Act
        var response = await _client.GetAsync(CategoryRoutes.GetById.Replace("{id:guid}", category.Id.ToString()));

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<Result<CategoryResponse>>();

        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.Data.ShouldNotBeNull();
        result.Data.Id.ShouldBe(category.Id);
        result.Data.Name.ShouldBe(category.Name);
    }

    [Fact]
    public async Task GetById_Should_ReturnCategoryFromCache_When_CacheHit()
    {
        // Arrange
        var categoryDto = await SeedCacheAsync();

        // Act
        var response = await _client.GetAsync(CategoryRoutes.GetById.Replace("{id:guid}", categoryDto.Id.ToString()));

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<Result<CategoryResponse>>();

        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.Data.ShouldNotBeNull();
        result.Data.Name.ShouldBe(categoryDto.Name);
        result.Data.ImageUrl.ShouldBe(categoryDto.ImageUrl);
    }

    [Fact]
    public async Task GetById_Should_ReturnConsistentResults_When_CalledConcurrently()
    {
        // Arrange
        var categorys = await SeedDatabaseRangeAsync(10);
        var ids = categorys.Select(x => x.Id).ToList();
        int concurrentCalls = 10;
        var rnd = new Random();
        var tasks = new List<Task<HttpResponseMessage>>();

        // Act
        for (int i = 0; i < concurrentCalls; i++)
        {
            var randomId = ids[rnd.Next(ids.Count)];
            var task = _client.GetAsync(CategoryRoutes.GetById.Replace("{id:guid}", randomId.ToString()));
            tasks.Add(task);
        }

        await Task.WhenAll(tasks);

        // Assert
        foreach (var task in tasks)
        {
            var response = await task;
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<Result<CategoryResponse>>();

            result.ShouldNotBeNull();
            result.IsSuccess.ShouldBeTrue();
            result.StatusCode.ShouldBe(HttpStatusCode.OK);
            result.Data.ShouldNotBeNull();
        }
    }

    private async Task<Category> SeedDatabaseAsync()
    {
        using var scope = _factory.Services.CreateScope();

        var category = new CategoryFaker().Generate();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        dbContext.Categories.Add(category);
        await dbContext.SaveChangesAsync();

        return category;
    }

    private async Task<IEnumerable<Category>> SeedDatabaseRangeAsync(int count = 10)
    {
        using var scope = _factory.Services.CreateScope();

        var categorys = new CategoryFaker().Generate(count);
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await dbContext.Categories.AddRangeAsync(categorys);
        await dbContext.SaveChangesAsync();

        return categorys;
    }

    private async Task<CategoryResponse> SeedCacheAsync()
    {
        using var scope = _factory.Services.CreateScope();

        var category = new CategoryFaker().Generate();

        var cache = scope.ServiceProvider.GetRequiredService<ICacheService>();
        var key = CacheKeys.Category(category.Id.ToString());
        var categoryDto = category.ToDto();
        var result = Result<CategoryResponse>.Success(categoryDto);
        await cache.SetAsync(key, result, TimeSpan.FromMinutes(5));

        return categoryDto;
    }
}
