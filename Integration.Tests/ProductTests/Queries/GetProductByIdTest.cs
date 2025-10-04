using System.Net;
using System.Net.Http.Json;
using Api.Routing;
using Application.Abstraction.Services;
using Application.Common.Constants;
using Application.Common.Results;
using Application.Features.Brand.Queries.GetBrandById;
using Application.Features.Category.Queries.GetCategoryById;
using Application.Features.Product.Queries.GetProductById;
using ArchitectureTests.FakeData;
using Domain.Entities;
using Infrastructure.Data;
using Integration.Tests.Shared;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Integration.Tests.ProductTests.Queries;

[Collection(nameof(SharedTestCollection))]
public class GetProductByIdTest : IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public GetProductByIdTest(CustomWebApplicationFactory factory)
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
        var response = await _client.GetAsync(ProductRoutes.GetById.Replace("{id:guid}", id.ToString()));

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);

        var result = await response.Content.ReadFromJsonAsync<Result<ProductDto>>();

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
        var response = await _client.GetAsync(ProductRoutes.GetById.Replace("{id:guid}", wrongId.ToString()));

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetById_WhenCacheMiss_ShouldReturnProductFromDatabase()
    {
        // Arrange
        var product = await SeedDatabaseAsync();

        // Act
        var response = await _client.GetAsync(ProductRoutes.GetById.Replace("{id:guid}", product.Id.ToString()));

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<Result<ProductDto>>();

        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.Data.ShouldNotBeNull();
        result.Data.Id.ShouldBe(product.Id);
        result.Data.Name.ShouldBe(product.Name);
    }

    [Fact]
    public async Task GetById_WhenCacheHit_ShouldReturnProductFromCache()
    {
        // Arrange
        var product = await SeedCacheAsync();

        // Act
        var response = await _client.GetAsync(ProductRoutes.GetById.Replace("{id:guid}", product.Id.ToString()));

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<Result<ProductDto>>();

        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.Data.ShouldNotBeNull();
        result.Data.Name.ShouldBe(product.Name);
    }

    [Fact]
    public async Task GetById_WhenCalledConcurrently_ShouldReturnConsistentResults()
    {
        // Arrange
        var products = await SeedDatabaseRangeAsync(10);
        var ids = products.Select(x => x.Id).ToList();
        int concurrentCalls = 10;
        var rnd = new Random();
        var tasks = new List<Task<HttpResponseMessage>>();

        // Act
        for (int i = 0; i < concurrentCalls; i++)
        {
            var randomId = ids[rnd.Next(ids.Count)];
            var task = _client.GetAsync(ProductRoutes.GetById.Replace("{id:guid}", randomId.ToString()));
            tasks.Add(task);
        }

        await Task.WhenAll(tasks);

        // Assert
        foreach (var task in tasks)
        {
            var response = await task;
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<Result<ProductDto>>();

            result.ShouldNotBeNull();
            result.IsSuccess.ShouldBeTrue();
            result.StatusCode.ShouldBe(HttpStatusCode.OK);
            result.Data.ShouldNotBeNull();
        }
    }

    private async Task<Product> SeedDatabaseAsync()
    {
        using var scope = _factory.Services.CreateScope();

        var product = new ProductFaker().Generate();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var brand = new BrandFaker().Generate();
        dbContext.Brands.Add(brand);

        var Category = new CategoryFaker().Generate();
        dbContext.Categories.Add(Category);

        product.CategoryId = Category.Id;
        product.BrandId = brand.Id;

        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync();

        return product;
    }

    private async Task<IEnumerable<Product>> SeedDatabaseRangeAsync(int count = 10)
    {
        using var scope = _factory.Services.CreateScope();

        var products = new ProductFaker().Generate(count);
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

    private async Task<ProductDto> SeedCacheAsync()
    {
        using var scope = _factory.Services.CreateScope();

        var product = new ProductFaker().Generate();

        var productDto = ToDto(product);

        var cache = scope.ServiceProvider.GetRequiredService<ICacheService>();
        var key = CacheKeys.Product(productDto.Id.ToString());
        var result = Result<ProductDto>.Success(productDto);
        await cache.SetAsync(key, result, TimeSpan.FromMinutes(5));

        return productDto;
    }

    private ProductDto ToDto(Product entity)
    {
        return new ProductDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            Thumbnail = entity.Thumbnail,
            Category = new CategoryDto
            {
                Id = Guid.NewGuid(),
                Name = "Category Name",
                ImageUrl = "ImageUrl"
            },
            Brand = new BrandDto
            {
                Id = Guid.NewGuid(),
                Name = "Brand Name",
                ImageUrl = "ImageUrl"
            },
            Tags = [],
            Discount = entity.Discount,
            Price = entity.Price,
            Rating = entity.Rating
        };
    }
}
