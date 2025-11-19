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
public class GetCategoryByIdTest(CustomWebApplicationFactory factory) : IAsyncLifetime
{
    private readonly HttpClient _client = factory.HttpClient;

    public async Task InitializeAsync() => await factory.ResetDatabaseAsync();

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task GetById_Should_ReturnFailure_When_NotFound()
    {
        // Arrange
        var id = Guid.CreateVersion7();

        // Act
        HttpResponseMessage response = await _client.GetAsync(
            CategoryRoutes.GetById.Replace("{id:guid}", id.ToString())
        );

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);

        Result<CategoryResponse>? result = await response.Content.ReadFromJsonAsync<Result<CategoryResponse>>();

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
        const string WrongId = "abc";

        // Act
        HttpResponseMessage response = await _client.GetAsync(CategoryRoutes.GetById.Replace("{id:guid}", WrongId));

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetById_Should_ReturnCategoryFromDatabase_When_CacheMiss()
    {
        // Arrange
        Category category = await SeedDatabaseAsync();

        // Act
        HttpResponseMessage response = await _client.GetAsync(
            CategoryRoutes.GetById.Replace("{id:guid}", category.Id.ToString())
        );

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        Result<CategoryResponse>? result = await response.Content.ReadFromJsonAsync<Result<CategoryResponse>>();

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
        CategoryResponse categoryDto = await SeedCacheAsync();

        // Act
        HttpResponseMessage response = await _client.GetAsync(
            CategoryRoutes.GetById.Replace("{id:guid}", categoryDto.Id.ToString())
        );

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        Result<CategoryResponse>? result = await response.Content.ReadFromJsonAsync<Result<CategoryResponse>>();

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
        IEnumerable<Category> categories = await SeedDatabaseRangeAsync();
        var ids = categories.Select(x => x.Id).ToList();
        const int ConcurrentCalls = 10;
        var rnd = new Random();
        var tasks = new List<Task<HttpResponseMessage>>();

        // Act
        for(var i = 0; i < ConcurrentCalls; i++)
        {
            Guid randomId = ids[rnd.Next(ids.Count)];
            Task<HttpResponseMessage> task = _client.GetAsync(
                CategoryRoutes.GetById.Replace("{id:guid}", randomId.ToString())
            );
            tasks.Add(task);
        }

        await Task.WhenAll(tasks);

        // Assert
        foreach(Task<HttpResponseMessage> task in tasks)
        {
            HttpResponseMessage response = await task;
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            Result<CategoryResponse>? result = await response.Content.ReadFromJsonAsync<Result<CategoryResponse>>();

            result.ShouldNotBeNull();
            result.IsSuccess.ShouldBeTrue();
            result.StatusCode.ShouldBe(HttpStatusCode.OK);
            result.Data.ShouldNotBeNull();
        }
    }

    private async Task<Category> SeedDatabaseAsync()
    {
        using IServiceScope scope = factory.Services.CreateScope();

        Category category = new CategoryFaker().Generate();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        dbContext.Categories.Add(category);
        await dbContext.SaveChangesAsync();

        return category;
    }

    private async Task<IEnumerable<Category>> SeedDatabaseRangeAsync(int count = 10)
    {
        using IServiceScope scope = factory.Services.CreateScope();

        List<Category> categories = new CategoryFaker().Generate(count);
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await dbContext.Categories.AddRangeAsync(categories);
        await dbContext.SaveChangesAsync();

        return categories;
    }

    private async Task<CategoryResponse> SeedCacheAsync()
    {
        using IServiceScope scope = factory.Services.CreateScope();

        Category category = new CategoryFaker().Generate();

        ICacheService cache = scope.ServiceProvider.GetRequiredService<ICacheService>();
        var key = CacheKeys.Category(category.Id.ToString());
        CategoryResponse categoryDto = category.ToDto();
        var result = Result<CategoryResponse>.Success(categoryDto);
        await cache.SetAsync(key, result, TimeSpan.FromMinutes(5));

        return categoryDto;
    }
}
