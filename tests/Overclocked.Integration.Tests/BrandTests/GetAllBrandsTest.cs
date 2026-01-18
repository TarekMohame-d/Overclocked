using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Overclocked.Api.Routing;
using Overclocked.Application.Abstractions.Services;
using Overclocked.Application.Common.Constants;
using Overclocked.Application.Features.BrandUseCases.DTOs.Responses;
using Overclocked.Application.Features.BrandUseCases.Mapping;
using Overclocked.Architecture.Tests.FakeData;
using Overclocked.Domain.BrandAggregate;
using Overclocked.Infrastructure.Persistence;
using Overclocked.Integration.Tests.Shared;
using Shouldly;

namespace Overclocked.Integration.Tests.BrandTests;

public class GetAllBrandsTest(ApiTestFixture fixture) : IAsyncLifetime
{
    private readonly HttpClient _client = fixture.HttpClient;

    public async ValueTask InitializeAsync() => await fixture.ResetDatabaseAsync();

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    [Fact]
    public async Task GetAll_Should_ReturnFromDatabase_When_ThereIsDataAndCacheMiss()
    {
        // Arrange
        IEnumerable<Brand> brands = await SeedDatabaseAsync();

        // Act
        HttpResponseMessage response = await _client.GetAsync(BrandRoutes.GetAll);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        IEnumerable<BrandListResponse>? result = await response.Content.ReadFromJsonAsync<IEnumerable<BrandListResponse>>();

        result.ShouldNotBeNull();
        result.ShouldNotBeEmpty();
        result.Count().ShouldBe(brands.Count());
    }

    [Fact]
    public async Task GetAll_Should_ReturnFromCache_When_ThereIsDataAndCacheHit()
    {
        // Arrange
        IEnumerable<BrandListResponse> brandListDtos = await SeedCacheAsync();

        // Act
        HttpResponseMessage response = await _client.GetAsync(BrandRoutes.GetAll);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        IEnumerable<BrandListResponse>? result = await response.Content.ReadFromJsonAsync<IEnumerable<BrandListResponse>>();

        result.ShouldNotBeNull();
        result.ShouldNotBeEmpty();
        result.Count().ShouldBe(brandListDtos.Count());
    }

    [Fact]
    public async Task GetAll_Should_ReturnEmptyList_When_ThereIsNoData()
    {
        // Arrange

        // Act
        HttpResponseMessage response = await _client.GetAsync(BrandRoutes.GetAll);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        IEnumerable<BrandListResponse>? result = await response.Content.ReadFromJsonAsync<IEnumerable<BrandListResponse>>();

        result.ShouldNotBeNull();
        result.ShouldBeEmpty();
        result.Count().ShouldBe(0);
    }

    [Fact]
    public async Task GetAll_Should_ReturnConsistentResults_When_CalledConcurrently()
    {
        // Arrange
        IEnumerable<Brand> brands = await SeedDatabaseAsync();

        const int ConcurrentCalls = 10;
        var tasks = new List<Task<HttpResponseMessage>>();

        // Act
        for (var i = 0; i < ConcurrentCalls; i++)
            tasks.Add(_client.GetAsync(BrandRoutes.GetAll));

        await Task.WhenAll(tasks);

        // Assert
        foreach (Task<HttpResponseMessage> task in tasks)
        {
            HttpResponseMessage response = await task;
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            IEnumerable<BrandListResponse>? result = await response.Content.ReadFromJsonAsync<IEnumerable<BrandListResponse>>();

            result.ShouldNotBeNull();
            result.ShouldNotBeEmpty();
            result.Count().ShouldBe(brands.Count());
        }
    }

    private async Task<IEnumerable<Brand>> SeedDatabaseAsync()
    {
        using IServiceScope scope = fixture.Services.CreateScope();

        List<Brand> brands = new BrandFaker().Generate(10);
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await dbContext.Brands.AddRangeAsync(brands);
        await dbContext.SaveChangesAsync();

        return brands;
    }

    private async Task<IEnumerable<BrandListResponse>> SeedCacheAsync()
    {
        using IServiceScope scope = fixture.Services.CreateScope();

        List<Brand> brands = new BrandFaker().Generate(10);

        ICacheService cache = scope.ServiceProvider.GetRequiredService<ICacheService>();
        var key = CacheKeys.AllBrands;
        IEnumerable<BrandListResponse> brandListResponses = brands.ToDto();

        await cache.SetAsync(key, brandListResponses, TimeSpan.FromMinutes(5));

        return brandListResponses;
    }
}
