using Api.Common.Routing;
using Application.Abstraction.Services;
using Application.Common.Constants;
using Application.Common.Results;
using Application.Features.Brand.Mapping;
using Application.Features.Brand.Queries.GetBrandById;
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
public class GetByIdBrandTests : IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public GetByIdBrandTests(CustomWebApplicationFactory factory)
    {
        _client = factory.HttpClient;
        _factory = factory;
    }

    public async Task InitializeAsync() => await _factory.ResetDatabaseAsync();
    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task GetById_WhenNotFound_ShouldReturnFailure()
    {
        // Arrange
        Guid id = Guid.CreateVersion7();

        // Act
        var response = await _client.GetAsync(BrandRoutes.GetById.Replace("{id:guid}", id.ToString()));

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);

        var result = await response.Content.ReadFromJsonAsync<Result<BrandDto>>();

        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeFalse();
        result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        result.Data.ShouldBeNull();
        result.Error.ShouldNotBeNull();
        result.Error.Type.ShouldBe(ErrorType.NotFound);
    }

    [Fact]
    public async Task GetById_WhenIdIsMalformedGuid_ShouldReturnNotFound()
    {
        // Arrange
        var wrongId = "abc";

        // Act
        var response = await _client.GetAsync(BrandRoutes.GetById.Replace("{id:guid}", wrongId.ToString()));

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetById_WhenCacheMiss_ShouldReturnBrandFromDatabase()
    {
        // Arrange
        var brand = await SeedDatabaseAsync();

        // Act
        var response = await _client.GetAsync(BrandRoutes.GetById.Replace("{id:guid}", brand.Id.ToString()));

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<Result<BrandDto>>();

        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.Data.ShouldNotBeNull();
        result.Data.Id.ShouldBe(brand.Id);
        result.Data.Name.ShouldBe(brand.Name);
    }

    [Fact]
    public async Task GetById_WhenCacheHit_ShouldReturnBrandFromCache()
    {
        // Arrange
        var brandDto = await SeedCacheAsync();

        // Act
        var response = await _client.GetAsync(BrandRoutes.GetById.Replace("{id:guid}", brandDto.Id.ToString()));

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<Result<BrandDto>>();

        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.Data.ShouldNotBeNull();
        result.Data.Name.ShouldBe(brandDto.Name);
        result.Data.ImageUrl.ShouldBe(brandDto.ImageUrl);
    }

    [Fact]
    public async Task GetById_WhenCalledConcurrently_ShouldReturnConsistentResults()
    {
        // Arrange
        var brands = await SeedDatabaseRangeAsync(10);
        var ids = brands.Select(x => x.Id).ToList();
        int concurrentCalls = 10;
        var rnd = new Random();
        var tasks = new List<Task<HttpResponseMessage>>();

        // Act
        for (int i = 0; i < concurrentCalls; i++)
        {
            var randomId = ids[rnd.Next(ids.Count)];
            var task = _client.GetAsync(BrandRoutes.GetById.Replace("{id:guid}", randomId.ToString()));
            tasks.Add(task);
        }

        await Task.WhenAll(tasks);

        // Assert
        foreach (var task in tasks)
        {
            var response = await task;
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<Result<BrandDto>>();

            result.ShouldNotBeNull();
            result.IsSuccess.ShouldBeTrue();
            result.StatusCode.ShouldBe(HttpStatusCode.OK);
            result.Data.ShouldNotBeNull();
        }
    }

    private async Task<Brand> SeedDatabaseAsync()
    {
        using var scope = _factory.Services.CreateScope();

        var brand = new BrandFaker().Generate();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        dbContext.Brands.Add(brand);
        await dbContext.SaveChangesAsync();

        return brand;
    }

    private async Task<IEnumerable<Brand>> SeedDatabaseRangeAsync(int count = 10)
    {
        using var scope = _factory.Services.CreateScope();

        var brands = new BrandFaker().Generate(count);
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await dbContext.Brands.AddRangeAsync(brands);
        await dbContext.SaveChangesAsync();

        return brands;
    }

    private async Task<BrandDto> SeedCacheAsync()
    {
        using var scope = _factory.Services.CreateScope();

        var brand = new BrandFaker().Generate();

        var cache = scope.ServiceProvider.GetRequiredService<ICacheService>();
        var key = CacheKeys.Brand(brand.Id.ToString());
        var brandDto = brand.ToDto();
        Result<BrandDto> result = Result<BrandDto>.Success(brandDto);
        await cache.SetAsync(key, result);

        return brandDto;
    }
}
