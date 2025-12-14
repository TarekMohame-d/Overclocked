using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Overclocked.Api.Routing;
using Overclocked.Application.Abstractions.Services;
using Overclocked.Application.Brand.Mapping;
using Overclocked.Application.Common.Constants;
using Overclocked.Architecture.Tests.FakeData;
using Overclocked.Contracts.Brand;
using Overclocked.Domain.BrandAggregate;
using Overclocked.Infrastructure.Persistence;
using Overclocked.Integration.Tests.Shared;
using Shouldly;

namespace Overclocked.Integration.Tests.BrandTests;

[Collection(nameof(SharedTestCollection))]
public class GetBrandByIdTest(CustomWebApplicationFactory factory) : IAsyncLifetime
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
        HttpResponseMessage response = await _client.GetAsync(BrandRoutes.GetById.Replace("{id:guid}", id.ToString()));

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetById_Should_ReturnNotFound_When_IdIsMalformedGuid()
    {
        // Arrange
        const string WrongId = "abc";

        // Act
        HttpResponseMessage response = await _client.GetAsync(BrandRoutes.GetById.Replace("{id:guid}", WrongId));

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetById_Should_ReturnBrandFromDatabase_When_CacheMiss()
    {
        // Arrange
        Brand brand = await SeedDatabaseAsync();

        // Act
        HttpResponseMessage response = await _client.GetAsync(
            BrandRoutes.GetById.Replace("{id:guid}", brand.Id.Value.ToString()));

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        BrandResponse? result = await response.Content.ReadFromJsonAsync<BrandResponse>();

        result.ShouldNotBeNull();
        result.Id.ShouldBe(brand.Id);
        result.Name.ShouldBe(brand.Name);
    }

    [Fact]
    public async Task GetById_Should_ReturnBrandFromCache_When_CacheHit()
    {
        // Arrange
        BrandResponse brandDto = await SeedCacheAsync();

        // Act
        HttpResponseMessage response = await _client.GetAsync(
            BrandRoutes.GetById.Replace("{id:guid}", brandDto.Id.ToString()));

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        BrandResponse? result = await response.Content.ReadFromJsonAsync<BrandResponse>();

        result.ShouldNotBeNull();
        result.Id.ShouldBe(brandDto.Id);
        result.Name.ShouldBe(brandDto.Name);
    }

    [Fact]
    public async Task GetById_Should_ReturnConsistentResults_When_CalledConcurrently()
    {
        // Arrange
        IEnumerable<Brand> brands = await SeedDatabaseRangeAsync();
        var ids = brands.Select(x => x.Id).ToList();
        const int ConcurrentCalls = 10;
        var rnd = new Random();
        var tasks = new List<Task<HttpResponseMessage>>();

        // Act
        for(var i = 0; i < ConcurrentCalls; i++)
        {
            Guid randomId = ids[rnd.Next(ids.Count)];
            Task<HttpResponseMessage> task = _client.GetAsync(
                BrandRoutes.GetById.Replace("{id:guid}", randomId.ToString())
            );
            tasks.Add(task);
        }

        await Task.WhenAll(tasks);

        // Assert
        foreach(Task<HttpResponseMessage> task in tasks)
        {
            HttpResponseMessage response = await task;
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            BrandResponse? result = await response.Content.ReadFromJsonAsync<BrandResponse>();

            result.ShouldNotBeNull();
        }
    }

    private async Task<Brand> SeedDatabaseAsync()
    {
        using IServiceScope scope = factory.Services.CreateScope();

        Brand brand = new BrandFaker().Generate();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        dbContext.Brands.Add(brand);
        await dbContext.SaveChangesAsync();

        return brand;
    }

    private async Task<IEnumerable<Brand>> SeedDatabaseRangeAsync(int count = 10)
    {
        using IServiceScope scope = factory.Services.CreateScope();

        List<Brand> brands = new BrandFaker().Generate(count);
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await dbContext.Brands.AddRangeAsync(brands);
        await dbContext.SaveChangesAsync();

        return brands;
    }

    private async Task<BrandResponse> SeedCacheAsync()
    {
        using IServiceScope scope = factory.Services.CreateScope();

        Brand brand = new BrandFaker().Generate();

        ICacheService cache = scope.ServiceProvider.GetRequiredService<ICacheService>();
        var key = CacheKeys.Brand(brand.Id.Value.ToString());
        BrandResponse brandDto = brand.ToDto();

        await cache.SetAsync(key, brandDto, TimeSpan.FromMinutes(5));

        return brandDto;
    }
}
