using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Overclocked.Api.Routing;
using Overclocked.Architecture.Tests.FakeData;
using Overclocked.Domain.BrandAggregate;
using Overclocked.Domain.CategoryAggregate;
using Overclocked.Domain.ProductAggregate;
using Overclocked.Domain.ProductAggregate.Events;
using Overclocked.Domain.TagAggregate;
using Overclocked.Domain.UserAggregate.Enums;
using Overclocked.Infrastructure.Outbox;
using Overclocked.Infrastructure.Persistence;
using Overclocked.Integration.Tests.Shared;
using Shouldly;

namespace Overclocked.Integration.Tests.ProductTests;

[Collection(nameof(SharedTestCollection))]
public class DeleteProductTest(CustomWebApplicationFactory factory) : IAsyncLifetime
{
    private readonly HttpClient _client = factory.HttpClient;

    public async Task InitializeAsync()
    {
        await factory.ResetDatabaseAsync();

        var token = CustomWebApplicationFactory.GenerateJwtToken(
            permissions: [Permission.AddEditDelete.ToString()]
        );
        factory.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
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
    }

    [Fact]
    public async Task Delete_Should_ReturnSuccess_When_ProductExists()
    {
        // Arrange
        (Guid brandId, Guid categoryId) = await SeedDependantEntityAsync();

        Product product = await SeedDatabaseAsync(brandId, categoryId);

        // Act
        HttpResponseMessage response = await _client.DeleteAsync(
            ProductRoutes.Delete.Replace("{id:guid}", product.Id.Value.ToString()));

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);

        using IServiceScope scope = factory.Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        Product? deletedProduct = await dbContext.Products.FindAsync(product.Id);

        deletedProduct.ShouldBeNull();

        OutboxMessage? message = await dbContext.Set<OutboxMessage>()
            .FirstOrDefaultAsync(x => x.Type == nameof(ProductDeletedEvent));

        message.ShouldNotBeNull();
        message.ProcessedOnUtc.ShouldBeNull();
    }

    [Fact]
    public async Task Delete_Should_ReturnForbidden_When_UserDoesNotHavePermission()
    {
        // Arrange
        (Guid brandId, Guid categoryId) = await SeedDependantEntityAsync();

        Product product = await SeedDatabaseAsync(brandId, categoryId);

        var token = CustomWebApplicationFactory.GenerateJwtToken();
        factory.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        HttpResponseMessage response = await _client.DeleteAsync(
            ProductRoutes.Delete.Replace("{id:guid}", product.Id.Value.ToString()));

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
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

    private async Task<(Guid brandId, Guid categoryId)> SeedDependantEntityAsync()
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

        return (brand.Id.Value, category.Id.Value);
    }
}
