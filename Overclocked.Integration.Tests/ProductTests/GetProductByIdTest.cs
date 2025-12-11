using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Overclocked.Api.Routing;
using Overclocked.Application.Abstraction.Services;
using Overclocked.Application.Common.Constants;
using Overclocked.Architecture.Tests.FakeData;
using Overclocked.Contracts.Brand;
using Overclocked.Contracts.Category;
using Overclocked.Contracts.Product;
using Overclocked.Domain.BrandAggregate;
using Overclocked.Domain.CategoryAggregate;
using Overclocked.Domain.ProductAggregate;
using Overclocked.Domain.TagAggregate;
using Overclocked.Infrastructure.Persistence;
using Overclocked.Integration.Tests.Shared;
using Shouldly;

namespace Overclocked.Integration.Tests.ProductTests;

[Collection(nameof(SharedTestCollection))]
public class GetProductByIdTest(CustomWebApplicationFactory factory) : IAsyncLifetime
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
        HttpResponseMessage response = await _client
            .GetAsync(ProductRoutes.GetById.Replace("{id:guid}", id.ToString()));

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
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
    public async Task GetById_Should_ReturnProductFromDatabase_When_CacheMiss()
    {
        // Arrange
        (Brand brand, Category category) = await SeedDependantEntityAsync();
        Product product = await SeedDatabaseAsync(brand.Id.Value, category.Id.Value);

        // Act
        HttpResponseMessage response = await _client.GetAsync(
            ProductRoutes.GetById.Replace("{id:guid}", product.Id.Value.ToString()));

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        ProductResponse? result = await response.Content.ReadFromJsonAsync<ProductResponse>();

        result.ShouldNotBeNull();
        result.Id.ShouldBe(product.Id);
        result.Name.ShouldBe(product.Name);
        result.Brand.Name.ShouldBe(brand.Name);
        result.Category.Name.ShouldBe(category.Name);
    }

    [Fact]
    public async Task GetById_Should_ReturnProductFromCache_When_CacheHit()
    {
        // Arrange
        var productId = Guid.NewGuid();
        ProductResponse productResponse = await SeedCacheAsync(productId);

        // Act
        HttpResponseMessage response = await _client.GetAsync(
            ProductRoutes.GetById.Replace("{id:guid}", productId.ToString()));

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        ProductResponse? result = await response.Content.ReadFromJsonAsync<ProductResponse>();

        result.ShouldNotBeNull();
        result.Id.ShouldBe(productResponse.Id);
        result.Name.ShouldBe(productResponse.Name);
    }

    private async Task<Product> SeedDatabaseAsync(Guid brandId, Guid categoryId)
    {
        Product product = new ProductFaker(brandId, categoryId).Generate();

        using IServiceScope scope = factory.Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync();

        return product;
    }

    private async Task<ProductResponse> SeedCacheAsync(Guid id)
    {
        using IServiceScope scope = factory.Services.CreateScope();

        var productResponse = new ProductResponse
        {
            Id = id,
            Name = "TestProduct",
            Description = "TestDescription",
            Thumbnail = "TestThumbnail",
            Price = 100,
            Discount = 10,
            FinalPrice = 90,
            Rating = 5,
            ReviewCount = 10,
            Brand = new BrandResponse
            {
                Id = Guid.NewGuid(),
                Name = "TestBrand",
                ImageUrl = "TestImageUrl"
            },
            Category = new CategoryResponse
            {
                Id = Guid.NewGuid(),
                Name = "TestCategory",
                ImageUrl = "TestImageUrl"
            },
            Tags = [],
            Specifications = [],
            RatingsBreakdown = new Dictionary<int, int>()
        };

        ICacheService cache = scope.ServiceProvider.GetRequiredService<ICacheService>();
        var key = CacheKeys.Product(id.ToString());

        await cache.SetAsync(key, productResponse, TimeSpan.FromMinutes(5));

        return productResponse;
    }

    private async Task<(Brand brand, Category category)> SeedDependantEntityAsync()
    {
        Brand brand = new BrandFaker().Generate();
        Category category = new CategoryFaker().Generate();
        List<Tag> tags = new TagFaker().Generate(3);

        using IServiceScope scope = factory.Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        dbContext.Brands.Add(brand);
        dbContext.Categories.Add(category);

        await dbContext.Tags.AddRangeAsync(tags);
        await dbContext.SaveChangesAsync();

        return (brand, category);
    }
}
