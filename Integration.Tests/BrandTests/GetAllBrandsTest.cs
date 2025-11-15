using System.Net;
using System.Net.Http.Json;
using Api.Routing;
using Application.Abstraction.Services;
using Application.Common.Constants;
using Application.Common.Results;
using Application.Services.Brand.DTOs.Response;
using Application.Services.Brand.Mapping;
using ArchitectureTests.FakeData;
using Domain.Entities;
using Infrastructure.Data;
using Integration.Tests.Shared;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Integration.Tests.BrandTests;

[Collection(nameof(SharedTestCollection))]
public class GetAllBrandsTest(CustomWebApplicationFactory factory) : IAsyncLifetime
{
    private readonly HttpClient _client = factory.HttpClient;

    public async Task InitializeAsync() => await factory.ResetDatabaseAsync();
    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task GetAll_Should_ReturnFromDatabase_When_ThereIsDataAndCacheMiss()
    {
        // Arrange
        IEnumerable<Brand> brands = await SeedDatabaseAsync();

        // Act
        HttpResponseMessage response = await _client.GetAsync(BrandRoutes.GetAll);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        Result<IEnumerable<BrandListResponse>>? result =
            await response.Content.ReadFromJsonAsync<Result<IEnumerable<BrandListResponse>>>();

        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.Data.ShouldNotBeNull();
        result.Data.ShouldNotBeEmpty();
        result.Data.Count().ShouldBe(brands.Count());
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

        Result<IEnumerable<BrandListResponse>>? result =
            await response.Content.ReadFromJsonAsync<Result<IEnumerable<BrandListResponse>>>();

        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.Data.ShouldNotBeNull();
        result.Data.ShouldNotBeEmpty();
        result.Data.Count().ShouldBe(brandListDtos.Count());
    }

    [Fact]
    public async Task GetAll_Should_ReturnEmptyList_When_ThereIsNoData()
    {
        // Arrange

        // Act
        HttpResponseMessage response = await _client.GetAsync(BrandRoutes.GetAll);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        Result<IEnumerable<BrandListResponse>>? result =
            await response.Content.ReadFromJsonAsync<Result<IEnumerable<BrandListResponse>>>();

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

            Result<IEnumerable<BrandListResponse>>? result =
                await response.Content.ReadFromJsonAsync<Result<IEnumerable<BrandListResponse>>>();

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
        using IServiceScope scope = factory.Services.CreateScope();

        List<Brand> brands = new BrandFaker().Generate(10);
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await dbContext.Brands.AddRangeAsync(brands);
        await dbContext.SaveChangesAsync();

        return brands;
    }

    private async Task<IEnumerable<BrandListResponse>> SeedCacheAsync()
    {
        using IServiceScope scope = factory.Services.CreateScope();

        List<Brand> brands = new BrandFaker().Generate(10);

        ICacheService cache = scope.ServiceProvider.GetRequiredService<ICacheService>();
        var key = CacheKeys.AllBrands;
        IEnumerable<BrandListResponse> brandListResponses = brands.ToDto();
        var result = Result<IEnumerable<BrandListResponse>>.Success(brandListResponses);
        await cache.SetAsync(key, result, TimeSpan.FromMinutes(5));

        return brandListResponses;
    }
}
