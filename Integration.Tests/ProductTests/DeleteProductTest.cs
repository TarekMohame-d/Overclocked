using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Api.Routing;
using Application.Common.Results;
using ArchitectureTests.FakeData;
using Domain.Entities;
using Domain.StaticData;
using Infrastructure.Data;
using Integration.Tests.Shared;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Integration.Tests.ProductTests;

[Collection(nameof(SharedTestCollection))]
public class DeleteProductTest(CustomWebApplicationFactory factory) : IAsyncLifetime
{
    private readonly HttpClient _client = factory.HttpClient;

    public async Task InitializeAsync()
    {
        await factory.ResetDatabaseAsync();

        var token = CustomWebApplicationFactory
            .GenerateJwtToken(permissions: [PermissionType.AddEditDelete.ToString()]);
        factory.HttpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Delete_Should_ReturnFailure_When_ProductNotFound()
    {
        // Arrange
        var productId = Guid.NewGuid().ToString();

        // Act
        HttpResponseMessage response = await _client.DeleteAsync(ProductRoutes.Delete.Replace("{id:guid}", productId));

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);

        Result? result = await response.Content.ReadFromJsonAsync<Result>();

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
        Product product = await SeedDatabaseAsync();

        // Act
        HttpResponseMessage response =
            await _client.DeleteAsync(ProductRoutes.Delete.Replace("{id:guid}", product.Id.ToString()));

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        Result? result = await response.Content.ReadFromJsonAsync<Result>();

        using IServiceScope scope = factory.Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        Product? deletedProduct = await dbContext.Products.FindAsync(product.Id);

        deletedProduct.ShouldBeNull();

        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBeNull();
    }

    [Fact]
    public async Task Delete_Should_ReturnForbidden_When_UserDoesNotHavePermission()
    {
        // Arrange
        Product product = await SeedDatabaseAsync();

        var token = CustomWebApplicationFactory.GenerateJwtToken();
        factory.HttpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        // Act
        HttpResponseMessage response =
            await _client.DeleteAsync(ProductRoutes.Delete.Replace("{id:guid}", product.Id.ToString()));

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    private async Task<Product> SeedDatabaseAsync()
    {
        Product product = new ProductFaker().Generate();

        Brand brand = new BrandFaker().Generate();
        product.BrandId = brand.Id;

        Category category = new CategoryFaker().Generate();
        product.CategoryId = category.Id;

        using IServiceScope scope = factory.Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        dbContext.Brands.Add(brand);
        dbContext.Categories.Add(category);

        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync();

        return product;
    }
}
