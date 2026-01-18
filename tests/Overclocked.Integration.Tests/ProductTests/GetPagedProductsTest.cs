using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Overclocked.Api.Routing;
using Overclocked.Application.Abstractions.Services;
using Overclocked.Application.Common.Constants;
using Overclocked.Application.Features.BrandUseCases.DTOs.Responses;
using Overclocked.Application.Features.ProductUseCases.DTOs.Responses;
using Overclocked.Architecture.Tests.FakeData;
using Overclocked.Domain.BrandAggregate;
using Overclocked.Domain.CategoryAggregate;
using Overclocked.Domain.ProductAggregate;
using Overclocked.Domain.TagAggregate;
using Overclocked.Infrastructure.Persistence;
using Overclocked.Integration.Tests.Shared;
using Overclocked.SharedKernel;
using Shouldly;

namespace Overclocked.Integration.Tests.ProductTests;

[Collection(nameof(IntegrationTestCollection))]
public class GetPagedProductsTest(IntegrationTestWebAppFactory factory) : IAsyncLifetime
{
    private readonly HttpClient _client = factory.HttpClient;

    public async Task InitializeAsync() => await factory.ResetDatabaseAsync();

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task GetPagedProducts_Should_ReturnEmptyItems_When_NotFound()
    {
        // Arrange
        var searchTerm = "test";
        var page = 1;
        var pageSize = 10;
        var url = $"{ProductRoutes.GetPaged}?Page={page}&PageSize={pageSize}&SearchTerm={searchTerm}&SortBy=name&Direction=asc";

        // Act
        HttpResponseMessage response = await _client.GetAsync(url);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        PagedResult<ProductPagedResponse>? result = await response.Content.ReadFromJsonAsync<PagedResult<ProductPagedResponse>>();

        result.ShouldNotBeNull();
        result.Items.ShouldBeEmpty();
        result.HasNextPage.ShouldBeFalse();
        result.HasPreviousPage.ShouldBeFalse();
    }

    [Fact]
    public async Task GetPagedProducts_Should_ReturnProductsFromDatabase_When_CacheMiss()
    {
        // Arrange
        (Brand brand, Category category, List<Guid> tags) = await SeedDependantEntityAsync();
        Product product = await SeedDatabaseAsync(brand.Id.Value, category.Id.Value, tags);

        var searchTerm = product.Name;
        var page = 1;
        var pageSize = 10;
        var url = $"{ProductRoutes.GetPaged}?Page={page}&PageSize={pageSize}&SearchTerm={searchTerm}&SortBy=name&Direction=asc";

        // Act
        HttpResponseMessage response = await _client.GetAsync(url);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        PagedResult<ProductPagedResponse>? result = await response.Content.ReadFromJsonAsync<PagedResult<ProductPagedResponse>>();

        result.ShouldNotBeNull();
        result.Items.ShouldNotBeEmpty();
        result.Items.First().Id.ShouldBe(product.Id.Value);
        result.HasNextPage.ShouldBeFalse();
        result.HasPreviousPage.ShouldBeFalse();
    }

    [Fact]
    public async Task GetPagedProducts_Should_ReturnProductsFromCache_When_CacheHit()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var page = 1;
        var pageSize = 10;
        var searchTerm = "Test";

        var cacheKey = CacheKeys.ProductPaged(
            page: page,
            pageSize: pageSize,
            sortBy: "name",
            direction: "asc",
            categoryId: Guid.Empty.ToString(),
            brandId: Guid.Empty.ToString(),
            tagId: Guid.Empty.ToString(),
            searchTerm: searchTerm
        );

        await SeedCacheAsync(productId, cacheKey);

        var url = $"{ProductRoutes.GetPaged}?page={page}&pageSize={pageSize}&searchTerm={searchTerm}&sortBy=name&direction=asc";

        // Act
        HttpResponseMessage response = await _client.GetAsync(url);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        PagedResult<ProductPagedResponse>? result = await response.Content.ReadFromJsonAsync<PagedResult<ProductPagedResponse>>();

        result.ShouldNotBeNull();
        result.Items.ShouldNotBeEmpty();
        result.Items.First().Id.ShouldBe(productId);
        result.HasNextPage.ShouldBeFalse();
        result.HasPreviousPage.ShouldBeFalse();
    }

    private async Task<Product> SeedDatabaseAsync(Guid brandId, Guid categoryId, List<Guid> tags)
    {
        Product product = new ProductFaker(brandId, categoryId, tags).Generate();

        using IServiceScope scope = factory.Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync();

        return product;
    }

    private async Task<PagedResult<ProductPagedResponse>> SeedCacheAsync(Guid id, string cacheKey)
    {
        using IServiceScope scope = factory.Services.CreateScope();

        IEnumerable<ProductPagedResponse> productPagedResponses =
        [
            new()
            {
                Id = id,
                Name = "TestProduct",
                Thumbnail = "TestThumbnail",
                Price = 100,
                Discount = 10,
                Rating = 5,
                ReviewCount = 10,
                Brand = new BrandResponse
                {
                    Id = Guid.NewGuid(),
                    Name = "TestBrand",
                    ImageUrl = "TestImageUrl",
                },
            },
        ];

        var result = PagedResult<ProductPagedResponse>.Create(productPagedResponses, 1, 10, 1);

        ICacheService cache = scope.ServiceProvider.GetRequiredService<ICacheService>();

        await cache.SetAsync(cacheKey, result, TimeSpan.FromMinutes(5));

        return result;
    }

    private async Task<(Brand brand, Category category, List<Guid> tags)> SeedDependantEntityAsync()
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

        return (brand, category, tags.ConvertAll(x => x.Id.Value));
    }
}
