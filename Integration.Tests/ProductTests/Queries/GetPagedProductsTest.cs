using System.Net;
using System.Net.Http.Json;
using Api.Routing;
using Application.Abstraction.Services;
using Application.Common.Constants;
using Application.Common.Results;
using Application.Features.Product.Queries.GetPagedProducts;
using ArchitectureTests.FakeData;
using Domain.Entities;
using Infrastructure.Data;
using Integration.Tests.Shared;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;


namespace Integration.Tests.ProductTests.Queries;

[Collection(nameof(SharedTestCollection))]
public class GetPagedProductsTest : IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public GetPagedProductsTest(CustomWebApplicationFactory factory)
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
        var products = await SeedDatabaseAsync();

        // Act
        var url = $"{ProductRoutes.GetAll}?Page=1&PageSize=10&SortBy=name_asc";
        var response = await _client.GetAsync(url);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<Result<PagedResult<ProductListDto>>>();

        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.Data.ShouldNotBeNull();
        result.Data.Items.ShouldNotBeEmpty();
        result.Data.Items.Count().ShouldBe(10);
    }

    [Fact]
    public async Task GetAll_WhenThereIsDataAndCacheHit_ShouldReturnFromCache()
    {
        // Arrange
        var productListDtos = await SeedCacheAsync();

        // Act
        var url = $"{ProductRoutes.GetAll}?Page=1&PageSize=20&SortBy=name_asc";
        var response = await _client.GetAsync(url);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<Result<PagedResult<ProductListDto>>>();

        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.Data.ShouldNotBeNull();
        result.Data.Items.ShouldNotBeEmpty();
        result.Data.Items.Count().ShouldBe(productListDtos.Count());
    }

    [Fact]
    public async Task GetAll_WhenThereIsNoData_ShouldReturnEmptyList()
    {
        // Arrange
        IEnumerable<Product> products = [];

        // Act
        var url = $"{ProductRoutes.GetAll}?Page=1&PageSize=10&SortBy=name_asc";
        var response = await _client.GetAsync(url);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<Result<PagedResult<ProductListDto>>>();

        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.Data.ShouldNotBeNull();
        result.Data.Items.ShouldBeEmpty();
        result.Data.Items.Count().ShouldBe(products.Count());
    }

    [Fact]
    public async Task GetAll_WhenCalledConcurrently_ShouldReturnConsistentResults()
    {
        // Arrange
        var products = await SeedDatabaseAsync();

        int concurrentCalls = 10;
        var tasks = new List<Task<HttpResponseMessage>>();

        // Act
        for (int i = 0; i < concurrentCalls; i++)
        {
            var url = $"{ProductRoutes.GetAll}?Page=1&PageSize=5&SortBy=name_asc";
            tasks.Add(_client.GetAsync(url));
        }

        await Task.WhenAll(tasks);

        // Assert
        foreach (var task in tasks)
        {
            var response = await task;
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<Result<PagedResult<ProductListDto>>>();

            result.ShouldNotBeNull();
            result.IsSuccess.ShouldBeTrue();
            result.StatusCode.ShouldBe(HttpStatusCode.OK);
            result.Data.ShouldNotBeNull();
            result.Data.Items.ShouldNotBeEmpty();
            result.Data.Items.Count().ShouldBe(5);
        }
    }

    private async Task<IEnumerable<Product>> SeedDatabaseAsync()
    {
        using var scope = _factory.Services.CreateScope();

        var products = new ProductFaker().Generate(20);
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var brand = new BrandFaker().Generate();
        dbContext.Brands.Add(brand);

        var Category = new CategoryFaker().Generate();
        dbContext.Categories.Add(Category);

        foreach (var product in products)
        {
            product.CategoryId = Category.Id;
            product.BrandId = brand.Id;
        }

        await dbContext.Products.AddRangeAsync(products);
        await dbContext.SaveChangesAsync();

        return products;
    }

    private async Task<IEnumerable<ProductListDto>> SeedCacheAsync()
    {
        using var scope = _factory.Services.CreateScope();

        var products = new ProductFaker().Generate(20);

        var cache = scope.ServiceProvider.GetRequiredService<ICacheService>();
        var key = CacheKeys.ProductPaged(1, 20, "name_asc");
        var productListDtos = products.Select(ToDto);
        PagedResult<ProductListDto> pagedResult = new PagedResult<ProductListDto>
        {
            Items = productListDtos.ToList(),
            PageNumber = 1,
            PageSize = 20,
            TotalItemCount = products.Count()
        };
        var result = Result<PagedResult<ProductListDto>>.Success(pagedResult);
        await cache.SetAsync(key, result, TimeSpan.FromMinutes(5));

        return productListDtos;
    }

    private ProductListDto ToDto(Product entity)
    {
        return new ProductListDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Thumbnail = entity.Thumbnail,
            Discount = entity.Discount,
            Price = entity.Price,
            Rating = entity.Rating
        };
    }
}
