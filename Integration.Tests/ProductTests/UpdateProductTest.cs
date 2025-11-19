using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
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
public class UpdateProductTest(CustomWebApplicationFactory factory) : IAsyncLifetime
{
    private readonly HttpClient _client = factory.HttpClient;

    public async Task InitializeAsync()
    {
        await factory.ResetDatabaseAsync();

        var token = CustomWebApplicationFactory.GenerateJwtToken(
            permissions: [PermissionType.AddEditDelete.ToString()]
        );
        factory.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Update_Should_ReturnFailure_When_ProductNotFound()
    {
        // Arrange
        const string Name = "AMD";
        var id = Guid.NewGuid().ToString();

        (Guid brandId, Guid categoryId, IEnumerable<Guid> tags) = await SeedDependantEntityAsync();
        StringContent form = CreateJsonContent(Name, brandId, categoryId, tags);

        // Act
        HttpResponseMessage response = await _client.PutAsync(ProductRoutes.Update.Replace("{id:guid}", id), form);

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
    public async Task Update_Should_ReturnSuccess_When_DataIsValid()
    {
        // Arrange
        (Guid brandId, Guid categoryId, IEnumerable<Guid> tags) = await SeedDependantEntityAsync();

        Product product = await SeedDatabaseAsync(brandId, categoryId);
        StringContent form = CreateJsonContent("New Name", brandId, categoryId, tags);

        // Act
        HttpResponseMessage response = await _client.PutAsync(
            ProductRoutes.Update.Replace("{id:guid}", product.Id.ToString()),
            form
        );

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        Result? result = await response.Content.ReadFromJsonAsync<Result>();

        using IServiceScope scope = factory.Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        Product? updatedProduct = await dbContext.Products.FindAsync(product.Id);
        updatedProduct.ShouldNotBeNull();
        updatedProduct.Name.ShouldBe("New Name");

        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBeNull();
    }

    [Fact]
    public async Task Update_Should_ReturnForbidden_When_UserDoesNotHavePermission()
    {
        // Arrange
        (Guid brandId, Guid categoryId, IEnumerable<Guid> tags) = await SeedDependantEntityAsync();

        Product product = await SeedDatabaseAsync(brandId, categoryId);
        StringContent form = CreateJsonContent("New Name", brandId, categoryId, tags);

        var token = CustomWebApplicationFactory.GenerateJwtToken();
        factory.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        HttpResponseMessage response = await _client.PutAsync(
            ProductRoutes.Update.Replace("{id:guid}", product.Id.ToString()),
            form
        );

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    private async Task<Product> SeedDatabaseAsync(Guid brandId, Guid categoryId)
    {
        Product product = new ProductFaker().Generate();

        product.BrandId = brandId;
        product.CategoryId = categoryId;

        using IServiceScope scope = factory.Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync();

        return product;
    }

    private async Task<(Guid brandId, Guid categoryId, IEnumerable<Guid> tags)> SeedDependantEntityAsync()
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

        return (brand.Id, category.Id, tags.Select(x => x.Id));
    }

    private static StringContent CreateJsonContent(string name, Guid brandId, Guid categoryId, IEnumerable<Guid> tags)
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
                "https://res.cloudinary.com/over-clocked/image2.png",
            },
            Specification = new[] { new { Name = "Name", Value = "Value" } },
        };

        var json = JsonSerializer.Serialize(payload);
        return new StringContent(json, Encoding.UTF8, "application/json");
    }
}
