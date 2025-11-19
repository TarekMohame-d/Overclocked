using System.Net;
using System.Net.Http.Json;
using Api.Routing;
using Application.Abstraction.Services;
using Application.Common.Constants;
using Application.Common.Results;
using Application.Services.Brand.DTOs.Response;
using Application.Services.Product.DTOs.Response;
using ArchitectureTests.FakeData;
using Domain.Entities;
using Infrastructure.Data;
using Integration.Tests.Shared;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Integration.Tests.ProductTests;

[Collection(nameof(SharedTestCollection))]
public class GetPagedProductsTest(CustomWebApplicationFactory factory) : IAsyncLifetime
{
    private readonly HttpClient _client = factory.HttpClient;

    public async Task InitializeAsync() => await factory.ResetDatabaseAsync();

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task GetPagedProducts_Should_ReturnFromDatabase_When_ThereIsDataAndCacheMiss()
    {
        // Arrange
        IEnumerable<Product> products = await SeedDatabaseAsync();

        // Act
        const string Url = $"{ProductRoutes.GetAll}?Page=1&PageSize=10&SortBy=name&Direction=asc";
        HttpResponseMessage response = await _client.GetAsync(Url);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        Result<PagedResult<ProductListResponse>>? result = await response.Content.ReadFromJsonAsync<
            Result<PagedResult<ProductListResponse>>
        >();

        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.Data.ShouldNotBeNull();
        result.Data.Items.ShouldNotBeEmpty();
        result.Data.Items.Count.ShouldBe(10);
    }

    [Fact]
    public async Task GetPagedProducts_Should_ReturnFromCache_When_ThereIsDataAndCacheHit()
    {
        // Arrange
        IEnumerable<ProductListResponse> productListDtos = await SeedCacheAsync();

        // Act
        const string Url = $"{ProductRoutes.GetAll}?Page=1&PageSize=20";
        HttpResponseMessage response = await _client.GetAsync(Url);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        Result<PagedResult<ProductListResponse>>? result = await response.Content.ReadFromJsonAsync<
            Result<PagedResult<ProductListResponse>>
        >();

        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.Data.ShouldNotBeNull();
        result.Data.Items.ShouldNotBeEmpty();
        result.Data.Items.Count.ShouldBe(productListDtos.Count());
    }

    [Fact]
    public async Task GetPagedProducts_Should_ReturnEmptyList_When_ThereIsNoData()
    {
        // Arrange
        IEnumerable<Product> products = [];

        // Act
        var url = $"{ProductRoutes.GetAll}?Page=1&PageSize=10&SortBy=name&Direction=asc";
        HttpResponseMessage response = await _client.GetAsync(url);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        Result<PagedResult<ProductListResponse>>? result = await response.Content.ReadFromJsonAsync<
            Result<PagedResult<ProductListResponse>>
        >();

        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.Data.ShouldNotBeNull();
        result.Data.Items.ShouldBeEmpty();
        result.Data.Items.Count.ShouldBe(products.Count());
    }

    [Fact]
    public async Task GetPagedProducts_Should_ReturnConsistentResults_When_CalledConcurrently()
    {
        // Arrange
        IEnumerable<Product> products = await SeedDatabaseAsync();

        const int ConcurrentCalls = 10;
        var tasks = new List<Task<HttpResponseMessage>>();

        // Act
        for(var i = 0; i < ConcurrentCalls; i++)
        {
            const string Url = $"{ProductRoutes.GetAll}?Page=1&PageSize=5&SortBy=name&Direction=asc";
            tasks.Add(_client.GetAsync(Url));
        }

        await Task.WhenAll(tasks);

        // Assert
        foreach(Task<HttpResponseMessage> task in tasks)
        {
            HttpResponseMessage response = await task;
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            Result<PagedResult<ProductListResponse>>? result = await response.Content.ReadFromJsonAsync<
                Result<PagedResult<ProductListResponse>>
            >();

            result.ShouldNotBeNull();
            result.IsSuccess.ShouldBeTrue();
            result.StatusCode.ShouldBe(HttpStatusCode.OK);
            result.Data.ShouldNotBeNull();
            result.Data.Items.ShouldNotBeEmpty();
            result.Data.Items.Count.ShouldBe(5);
        }
    }

    private async Task<IEnumerable<Product>> SeedDatabaseAsync()
    {
        using IServiceScope scope = factory.Services.CreateScope();

        List<Product> products = new ProductFaker().Generate(20);
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        Brand brand = new BrandFaker().Generate();
        dbContext.Brands.Add(brand);

        Category category = new CategoryFaker().Generate();
        dbContext.Categories.Add(category);

        foreach(Product product in products)
        {
            product.CategoryId = category.Id;
            product.BrandId = brand.Id;
        }

        await dbContext.Products.AddRangeAsync(products);
        await dbContext.SaveChangesAsync();

        return products;
    }

    private async Task<IEnumerable<ProductListResponse>> SeedCacheAsync()
    {
        using IServiceScope scope = factory.Services.CreateScope();

        List<Product> products = new ProductFaker().Generate(20);
        List<Brand> brands = new BrandFaker().Generate(20);

        ICacheService cache = scope.ServiceProvider.GetRequiredService<ICacheService>();
        var key = CacheKeys.ProductPaged(1, 20, "Id", "Asc", "all", "all", "all", "all");
        IEnumerable<ProductListResponse> productListResponses = [];

        for(var i = 0; i < products.Count; i++)
        {
            products[i].Brand = brands[i];
            products[i].BrandId = brands[i].Id;
            productListResponses = productListResponses.Append(ToDto(products[i], brands[i]));
        }

        var pagedResult = new PagedResult<ProductListResponse>
        {
            Items = productListResponses.ToList(),
            PageNumber = 1,
            PageSize = 20,
            TotalItemCount = products.Count,
        };
        var result = Result<PagedResult<ProductListResponse>>.Success(pagedResult);
        await cache.SetAsync(key, result, TimeSpan.FromMinutes(5));

        return productListResponses;
    }

    private static ProductListResponse ToDto(Product entity, Brand brand)
    {
        return new ProductListResponse
        {
            Id = entity.Id,
            Name = entity.Name,
            Thumbnail = entity.Thumbnail,
            Discount = entity.Discount,
            Price = entity.Price,
            Rating = entity.Rating,
            Brand = new BrandResponse
            {
                Id = brand.Id,
                Name = brand.Name,
                ImageUrl = brand.Image,
            },
        };
    }
}
