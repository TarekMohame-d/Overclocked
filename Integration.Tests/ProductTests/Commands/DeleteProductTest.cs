using ArchitectureTests.FakeData;
using Infrastructure.Data;
using Integration.Tests.Shared;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Domain.Entities;
using Application.Common.Results;
using Api.Routing;

namespace Integration.Tests.ProductTests.Commands;

[Collection(nameof(SharedTestCollection))]
public class DeleteProductTest : IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public DeleteProductTest(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.HttpClient;
    }

    public async Task InitializeAsync()
    {
        await _factory.ResetDatabaseAsync();

        var token = _factory.GenerateJwtToken();
        _factory.HttpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
    }
    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Delete_Should_ReturnFailure_When_ProductNotFound()
    {
        // Arrange
        var productId = Guid.NewGuid().ToString();

        // Act
        var response = await _client.DeleteAsync(ProductRoutes.Delete.Replace("{id:guid}", productId));

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);

        var result = await response.Content.ReadFromJsonAsync<Result>();

        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBeNull();
        result.Error.Type.ShouldBe(ErrorType.NotFound);
        result.Error.Description.ShouldBe("Product not found.");
    }

    [Fact]
    public async Task Delete_Should_ReturnSuccess_When_ProductExists()
    {
        // Arrange
        var product = await SeedDatabaseAsync();

        // Act
        var response = await _client.DeleteAsync(ProductRoutes.Delete.Replace("{id:guid}", product.Id.ToString()));

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<Result>();

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        Product? deletedProduct = await dbContext.Products.FindAsync(product.Id);

        deletedProduct.ShouldBeNull();

        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBeNull();
    }

    private async Task<Product> SeedDatabaseAsync()
    {
        var product = new ProductFaker().Generate();

        var brand = new BrandFaker().Generate();
        product.BrandId = brand.Id;

        var Category = new CategoryFaker().Generate();
        product.CategoryId = Category.Id;

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        dbContext.Brands.Add(brand);
        dbContext.Categories.Add(Category);

        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync();

        return product;
    }
}
