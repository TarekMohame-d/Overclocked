using System.Net;
using System.Net.Http.Json;
using Api.Routing;
using Application.Abstraction.Services;
using Application.Common.Constants;
using Application.Common.Results;
using Application.Services.Brand.DTOs.Response;
using Application.Services.Category.DTOs.Response;
using Application.Services.Product.DTOs.Response;
using ArchitectureTests.FakeData;
using Domain.Entities;
using Infrastructure.Data;
using Integration.Tests.Shared;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Integration.Tests.ProductTests;

[Collection(nameof(SharedTestCollection))]
public class GetProductByIdTest(CustomWebApplicationFactory factory) : IAsyncLifetime
{
    private readonly HttpClient _client = factory.HttpClient;

    public async Task InitializeAsync() => await factory.ResetDatabaseAsync();

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task GetById_Should_ReturnFailure_WhenNotFound()
    {
        // Arrange
        var id = Guid.CreateVersion7();

        // Act
        HttpResponseMessage response = await _client.GetAsync(
            ProductRoutes.GetById.Replace("{id:guid}", id.ToString())
        );

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);

        Result<ProductResponse>? result = await response.Content.ReadFromJsonAsync<Result<ProductResponse>>();

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
        HttpResponseMessage response = await _client.GetAsync(ProductRoutes.GetById.Replace("{id:guid}", WrongId));

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetById_Should_ReturnProductFromDatabase_WhenCacheMiss()
    {
        // Arrange
        Product product = await SeedDatabaseAsync();

        // Act
        HttpResponseMessage response = await _client.GetAsync(
            ProductRoutes.GetById.Replace("{id:guid}", product.Id.ToString())
        );

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        Result<ProductResponse>? result = await response.Content.ReadFromJsonAsync<Result<ProductResponse>>();

        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.Data.ShouldNotBeNull();
        result.Data.Id.ShouldBe(product.Id);
        result.Data.Name.ShouldBe(product.Name);
    }

    [Fact]
    public async Task GetById_Should_ReturnProductFromCache_When_CacheHit()
    {
        // Arrange
        ProductResponse product = await SeedCacheAsync();

        // Act
        HttpResponseMessage response = await _client.GetAsync(
            ProductRoutes.GetById.Replace("{id:guid}", product.Id.ToString())
        );

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        Result<ProductResponse>? result = await response.Content.ReadFromJsonAsync<Result<ProductResponse>>();

        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.Data.ShouldNotBeNull();
        result.Data.Name.ShouldBe(product.Name);
    }

    [Fact]
    public async Task GetById_Should_ReturnConsistentResults_When_CalledConcurrently()
    {
        // Arrange
        IEnumerable<Product> products = await SeedDatabaseRangeAsync();
        var ids = products.Select(x => x.Id).ToList();
        const int ConcurrentCalls = 10;
        var rnd = new Random();
        var tasks = new List<Task<HttpResponseMessage>>();

        // Act
        for(var i = 0; i < ConcurrentCalls; i++)
        {
            Guid randomId = ids[rnd.Next(ids.Count)];
            Task<HttpResponseMessage> task = _client.GetAsync(
                ProductRoutes.GetById.Replace("{id:guid}", randomId.ToString())
            );
            tasks.Add(task);
        }

        await Task.WhenAll(tasks);

        // Assert
        foreach(Task<HttpResponseMessage> task in tasks)
        {
            HttpResponseMessage response = await task;
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            Result<ProductResponse>? result = await response.Content.ReadFromJsonAsync<Result<ProductResponse>>();

            result.ShouldNotBeNull();
            result.IsSuccess.ShouldBeTrue();
            result.StatusCode.ShouldBe(HttpStatusCode.OK);
            result.Data.ShouldNotBeNull();
        }
    }

    private async Task<Product> SeedDatabaseAsync()
    {
        using IServiceScope scope = factory.Services.CreateScope();

        Product product = new ProductFaker().Generate();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        Brand brand = new BrandFaker().Generate();
        dbContext.Brands.Add(brand);

        Category category = new CategoryFaker().Generate();
        dbContext.Categories.Add(category);

        product.CategoryId = category.Id;
        product.BrandId = brand.Id;

        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync();

        return product;
    }

    private async Task<IEnumerable<Product>> SeedDatabaseRangeAsync(int count = 10)
    {
        using IServiceScope scope = factory.Services.CreateScope();

        List<Product> products = new ProductFaker().Generate(count);
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

    private async Task<ProductResponse> SeedCacheAsync()
    {
        using IServiceScope scope = factory.Services.CreateScope();

        Product product = new ProductFaker().Generate();

        ProductResponse productResponse = ToDto(product);

        ICacheService cache = scope.ServiceProvider.GetRequiredService<ICacheService>();
        var key = CacheKeys.Product(productResponse.Id.ToString());
        var result = Result<ProductResponse>.Success(productResponse);
        await cache.SetAsync(key, result, TimeSpan.FromMinutes(5));

        return productResponse;
    }

    private static ProductResponse ToDto(Product entity)
    {
        return new ProductResponse
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            Thumbnail = entity.Thumbnail,
            Category = new CategoryResponse
            {
                Id = Guid.NewGuid(),
                Name = "Category Name",
                ImageUrl = "ImageUrl",
            },
            Brand = new BrandResponse
            {
                Id = Guid.NewGuid(),
                Name = "Brand Name",
                ImageUrl = "ImageUrl",
            },
            Tags = [],
            Discount = entity.Discount,
            Price = entity.Price,
            Rating = entity.Rating,
            Specifications = [],
        };
    }
}
