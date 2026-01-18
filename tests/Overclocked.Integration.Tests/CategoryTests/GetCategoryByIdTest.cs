using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Overclocked.Api.Routing;
using Overclocked.Application.Abstractions.Services;
using Overclocked.Application.Common.Constants;
using Overclocked.Application.Features.CategoryUseCases.DTOs.Responses;
using Overclocked.Application.Features.CategoryUseCases.Mapping;
using Overclocked.Architecture.Tests.FakeData;
using Overclocked.Domain.CategoryAggregate;
using Overclocked.Infrastructure.Persistence;
using Overclocked.Integration.Tests.Shared;
using Shouldly;

namespace Overclocked.Integration.Tests.CategoryTests;

public class GetCategoryByIdTest(IntegrationTestWebAppFactory fixture) : IAsyncLifetime
{
    private readonly HttpClient _client = fixture.HttpClient;

    public async ValueTask InitializeAsync() => await fixture.ResetDatabaseAsync();

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    [Fact]
    public async Task GetById_Should_ReturnFailure_When_NotFound()
    {
        // Arrange
        var id = Guid.CreateVersion7();

        // Act
        HttpResponseMessage response = await _client.GetAsync(CategoryRoutes.GetById.Replace("{id:guid}", id.ToString()));

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
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
            CategoryRoutes.GetById.Replace("{id:guid}", category.Id.Value.ToString())
        );

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        CategoryResponse? result = await response.Content.ReadFromJsonAsync<CategoryResponse>();

        result.ShouldNotBeNull();
        result.Id.ShouldBe(category.Id.Value);
        result.Name.ShouldBe(category.Name);
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

        CategoryResponse? result = await response.Content.ReadFromJsonAsync<CategoryResponse>();

        result.ShouldNotBeNull();
        result.Id.ShouldBe(categoryDto.Id);
        result.Name.ShouldBe(categoryDto.Name);
    }

    [Fact]
    public async Task GetById_Should_ReturnConsistentResults_When_CalledConcurrently()
    {
        // Arrange
        IEnumerable<Category> categorys = await SeedDatabaseRangeAsync();
        var ids = categorys.Select(x => x.Id.Value).ToList();
        const int ConcurrentCalls = 10;
        var rnd = new Random();
        var tasks = new List<Task<HttpResponseMessage>>();

        // Act
        for (var i = 0; i < ConcurrentCalls; i++)
        {
            Guid randomId = ids[rnd.Next(ids.Count)];
            Task<HttpResponseMessage> task = _client.GetAsync(CategoryRoutes.GetById.Replace("{id:guid}", randomId.ToString()));
            tasks.Add(task);
        }

        await Task.WhenAll(tasks);

        // Assert
        foreach (Task<HttpResponseMessage> task in tasks)
        {
            HttpResponseMessage response = await task;
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            CategoryResponse? result = await response.Content.ReadFromJsonAsync<CategoryResponse>();

            result.ShouldNotBeNull();
        }
    }

    private async Task<Category> SeedDatabaseAsync()
    {
        using IServiceScope scope = fixture.Services.CreateScope();

        Category category = new CategoryFaker().Generate();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        dbContext.Categories.Add(category);
        await dbContext.SaveChangesAsync();

        return category;
    }

    private async Task<IEnumerable<Category>> SeedDatabaseRangeAsync(int count = 10)
    {
        using IServiceScope scope = fixture.Services.CreateScope();

        List<Category> categorys = new CategoryFaker().Generate(count);
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await dbContext.Categories.AddRangeAsync(categorys);
        await dbContext.SaveChangesAsync();

        return categorys;
    }

    private async Task<CategoryResponse> SeedCacheAsync()
    {
        using IServiceScope scope = fixture.Services.CreateScope();

        Category category = new CategoryFaker().Generate();

        ICacheService cache = scope.ServiceProvider.GetRequiredService<ICacheService>();
        var key = CacheKeys.Category(category.Id.Value.ToString());
        CategoryResponse categoryDto = category.ToDto();

        await cache.SetAsync(key, categoryDto, TimeSpan.FromMinutes(5));

        return categoryDto;
    }
}
