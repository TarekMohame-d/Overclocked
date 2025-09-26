using Api.Common.Routing;
using Application.Abstraction.Services;
using Application.Common.Constants;
using Application.Common.Results;
using Application.Features.Brand.Mapping;
using Application.Features.Brand.Queries.GetAllBrands;
using ArchitectureTests.FakeData;
using Infrastructure.Data;
using Integration.Tests.Shared;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Net;
using System.Net.Http.Json;
using Domain.Entities;

namespace Integration.Tests.BrandTests.Queries;

[Collection(nameof(SharedTestCollection))]
public class GetAllBrandTest : IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public GetAllBrandTest(CustomWebApplicationFactory factory)
    {
        _client = factory.HttpClient;
        _factory = factory;
    }

    public async Task InitializeAsync() => await _factory.ResetDatabaseAsync();
    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task GetAll_WhenThereIsDataAndCacheMiss_ShouldReturnFromDatabase()
    {
        // Arrange
        var brands = await SeedDatabaseAsync();

        // Act
        var response = await _client.GetAsync(BrandRoutes.GetAll);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<Result<IEnumerable<BrandListDto>>>();

        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.Data.ShouldNotBeNull();
        result.Data.ShouldNotBeEmpty();
        result.Data.Count().ShouldBe(brands.Count());
    }

    [Fact]
    public async Task GetAll_WhenThereIsDataAndCacheHit_ShouldReturnFromCache()
    {
        // Arrange
        var brandListDtos = await SeedCacheAsync();

        // Act
        var response = await _client.GetAsync(BrandRoutes.GetAll);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<Result<IEnumerable<BrandListDto>>>();

        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.Data.ShouldNotBeNull();
        result.Data.ShouldNotBeEmpty();
        result.Data.Count().ShouldBe(brandListDtos.Count());
    }

    [Fact]
    public async Task GetAll_WhenThereIsNoData_ShouldReturnEmptyList()
    {
        // Arrange
        IEnumerable<Brand> brands = [];

        // Act
        var response = await _client.GetAsync(BrandRoutes.GetAll);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<Result<IEnumerable<BrandListDto>>>();

        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.Data.ShouldNotBeNull();
        result.Data.ShouldBeEmpty();
        result.Data.Count().ShouldBe(brands.Count());
    }

    [Fact]
    public async Task GetAll_WhenCalledConcurrently_ShouldReturnConsistentResults()
    {
        // Arrange
        var brands = await SeedDatabaseAsync();

        int concurrentCalls = 10;
        var tasks = new List<Task<HttpResponseMessage>>();

        // Act
        for (int i = 0; i < concurrentCalls; i++)
        {
            tasks.Add(_client.GetAsync(BrandRoutes.GetAll));
        }

        await Task.WhenAll(tasks);

        // Assert
        foreach (var task in tasks)
        {
            var response = await task;
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<Result<IEnumerable<BrandListDto>>>();

            result.ShouldNotBeNull();
            result.IsSuccess.ShouldBeTrue();
            result.StatusCode.ShouldBe(HttpStatusCode.OK);
            result.Data.ShouldNotBeNull();
            result.Data.ShouldNotBeEmpty();
            result.Data.Count().ShouldBe(brands.Count());
        }
    }

    private async Task<IEnumerable<Brand>> SeedDatabaseAsync()
    {
        using var scope = _factory.Services.CreateScope();

        var brands = new BrandFaker().Generate(10);
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await dbContext.Brands.AddRangeAsync(brands);
        await dbContext.SaveChangesAsync();

        return brands;
    }

    private async Task<IEnumerable<BrandListDto>> SeedCacheAsync()
    {
        using var scope = _factory.Services.CreateScope();

        var brands = new BrandFaker().Generate(10);

        var cache = scope.ServiceProvider.GetRequiredService<ICacheService>();
        var key = CacheKeys.AllBrands;
        var brandListDtos = brands.ToDto();
        var result = Result<IEnumerable<BrandListDto>>.Success(brandListDtos);
        await cache.SetAsync(key, result, TimeSpan.FromMinutes(5));

        return brandListDtos;
    }
}
