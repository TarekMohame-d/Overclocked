using Infrastructure.Data;
using Integration.Tests.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Domain.Entities;
using Application.Common.Results;
using ArchitectureTests.FakeData;
using System.Text.Json;
using System.Text;
using Api.Routing;

namespace Integration.Tests.ProductTests;

[Collection(nameof(SharedTestCollection))]
public class UpdateProductTest : IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public UpdateProductTest(CustomWebApplicationFactory factory)
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
    public async Task Update_Should_ReturnFailure_When_ProductNotFound()
    {
        // Arrange
        var name = "AMD";
        var id = Guid.NewGuid().ToString();

        var ids = await SeedDependantEntityAsync();
        var form = CreateJsonContent(name, ids.brandId, ids.categoryId, ids.tags);

        // Act
        var response = await _client.PutAsync(ProductRoutes.Update.Replace("{id:guid}", id), form);

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
    public async Task Update_Should_ReturnSuccess_When_DataIsValid()
    {
        // Arrange
        var ids = await SeedDependantEntityAsync();

        var product = await SeedDatabaseAsync(ids.brandId, ids.categoryId);
        var form = CreateJsonContent("New Name", ids.brandId, ids.categoryId, ids.tags);

        // Act
        var response = await _client.PutAsync(ProductRoutes.Update.Replace("{id:guid}", product.Id.ToString()), form);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<Result>();

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        Product? updatedProduct = await dbContext.Products.FindAsync(product.Id);
        updatedProduct.ShouldNotBeNull();
        updatedProduct.Name.ShouldBe("New Name");

        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBeNull();
    }

    private async Task<Product> SeedDatabaseAsync(Guid brandId, Guid categoryId)
    {
        var product = new ProductFaker().Generate();

        product.BrandId = brandId;
        product.CategoryId = categoryId;

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync();

        return product;
    }

    private async Task<(Guid brandId, Guid categoryId, IEnumerable<Guid> tags)> SeedDependantEntityAsync()
    {
        var brand = new BrandFaker().Generate();
        var category = new CategoryFaker().Generate();
        var tags = new TagFaker().Generate(3);

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        dbContext.Brands.Add(brand);
        dbContext.Categories.Add(category);

        await dbContext.Tags.AddRangeAsync(tags);
        await dbContext.SaveChangesAsync();

        return (brand.Id, category.Id, tags.Select(x => x.Id));
    }

    private StringContent CreateJsonContent(string name, Guid brandId, Guid categoryId, IEnumerable<Guid> tags)
    {
        var payload = new
        {
            BrandId = brandId,
            CategoryId = categoryId,
            Name = name,
            Thumbnail = "https://res.cloudinary.com/over-clocked/image.png",
            Description = "Product Description",
            Price = 100,
            Stock = 10,
            Discount = 0,
            Tags = tags,
            Images = new List<string>
            {
                "https://res.cloudinary.com/over-clocked/image1.png",
                "https://res.cloudinary.com/over-clocked/image2.png"
            },
            Specification = new[]
            {
            new { Name = "Name", Value = "Value" }
            }
        };

        string json = JsonSerializer.Serialize(payload);
        return new StringContent(json, Encoding.UTF8, "application/json");
    }
}
