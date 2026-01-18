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

[Collection(nameof(IntegrationTestCollection))]
public class GetAllCategorysTest(IntegrationTestWebAppFactory factory) : IAsyncLifetime
{
    private readonly HttpClient _client = factory.HttpClient;

    public async Task InitializeAsync() => await factory.ResetDatabaseAsync();

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task GetAll_Should_ReturnFromDatabase_When_ThereIsDataAndCacheMiss()
    {
        // Arrange
        IEnumerable<Category> categorys = await SeedDatabaseAsync();

        // Act
        HttpResponseMessage response = await _client.GetAsync(CategoryRoutes.GetAll);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        IEnumerable<CategoryListResponse>? result = await response.Content.ReadFromJsonAsync<IEnumerable<CategoryListResponse>>();

        result.ShouldNotBeNull();
        result.ShouldNotBeEmpty();
        result.Count().ShouldBe(categorys.Count());
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

        IEnumerable<CategoryListResponse>? result = await response.Content.ReadFromJsonAsync<IEnumerable<CategoryListResponse>>();

        result.ShouldNotBeNull();
        result.ShouldNotBeEmpty();
        result.Count().ShouldBe(categoryListDtos.Count());
    }

    [Fact]
    public async Task GetAll_Should_ReturnEmptyList_When_ThereIsNoData()
    {
        // Arrange

        // Act
        HttpResponseMessage response = await _client.GetAsync(CategoryRoutes.GetAll);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        IEnumerable<CategoryListResponse>? result = await response.Content.ReadFromJsonAsync<IEnumerable<CategoryListResponse>>();

        result.ShouldNotBeNull();
        result.ShouldBeEmpty();
        result.Count().ShouldBe(0);
    }

    [Fact]
    public async Task GetAll_Should_ReturnConsistentResults_When_CalledConcurrently()
    {
        // Arrange
        IEnumerable<Category> categorys = await SeedDatabaseAsync();

        const int ConcurrentCalls = 10;
        var tasks = new List<Task<HttpResponseMessage>>();

        // Act
        for (var i = 0; i < ConcurrentCalls; i++)
            tasks.Add(_client.GetAsync(CategoryRoutes.GetAll));

        await Task.WhenAll(tasks);

        // Assert
        foreach (Task<HttpResponseMessage> task in tasks)
        {
            HttpResponseMessage response = await task;
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            IEnumerable<CategoryListResponse>? result = await response.Content.ReadFromJsonAsync<
                IEnumerable<CategoryListResponse>
            >();

            result.ShouldNotBeNull();
            result.ShouldNotBeEmpty();
            result.Count().ShouldBe(categorys.Count());
        }
    }

    private async Task<IEnumerable<Category>> SeedDatabaseAsync()
    {
        using IServiceScope scope = factory.Services.CreateScope();

        List<Category> categorys = new CategoryFaker().Generate(10);
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await dbContext.Categories.AddRangeAsync(categorys);
        await dbContext.SaveChangesAsync();

        return categorys;
    }

    private async Task<IEnumerable<CategoryListResponse>> SeedCacheAsync()
    {
        using IServiceScope scope = factory.Services.CreateScope();

        List<Category> categories = new CategoryFaker().Generate(10);

        ICacheService cache = scope.ServiceProvider.GetRequiredService<ICacheService>();
        var key = CacheKeys.AllCategories;
        IEnumerable<CategoryListResponse> categoryListResponses = categories.ToDto();

        await cache.SetAsync(key, categoryListResponses, TimeSpan.FromMinutes(5));

        return categoryListResponses;
    }
}
