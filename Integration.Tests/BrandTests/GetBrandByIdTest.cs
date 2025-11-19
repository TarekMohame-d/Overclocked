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

        Result<BrandResponse>? result = await response.Content.ReadFromJsonAsync<Result<BrandResponse>>();

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
            BrandRoutes.GetById.Replace("{id:guid}", brand.Id.ToString())
        );

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        Result<BrandResponse>? result = await response.Content.ReadFromJsonAsync<Result<BrandResponse>>();

        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.Data.ShouldNotBeNull();
        result.Data.Id.ShouldBe(brand.Id);
        result.Data.Name.ShouldBe(brand.Name);
    }

    [Fact]
    public async Task GetById_Should_ReturnBrandFromCache_When_CacheHit()
    {
        // Arrange
        BrandResponse brandDto = await SeedCacheAsync();

        // Act
        HttpResponseMessage response = await _client.GetAsync(
            BrandRoutes.GetById.Replace("{id:guid}", brandDto.Id.ToString())
        );

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        Result<BrandResponse>? result = await response.Content.ReadFromJsonAsync<Result<BrandResponse>>();

        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.Data.ShouldNotBeNull();
        result.Data.Name.ShouldBe(brandDto.Name);
        result.Data.ImageUrl.ShouldBe(brandDto.ImageUrl);
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

            Result<BrandResponse>? result = await response.Content.ReadFromJsonAsync<Result<BrandResponse>>();

            result.ShouldNotBeNull();
            result.IsSuccess.ShouldBeTrue();
            result.StatusCode.ShouldBe(HttpStatusCode.OK);
            result.Data.ShouldNotBeNull();
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
        var key = CacheKeys.Brand(brand.Id.ToString());
        BrandResponse brandDto = brand.ToDto();
        var result = Result<BrandResponse>.Success(brandDto);
        await cache.SetAsync(key, result, TimeSpan.FromMinutes(5));

        return brandDto;
    }
}
