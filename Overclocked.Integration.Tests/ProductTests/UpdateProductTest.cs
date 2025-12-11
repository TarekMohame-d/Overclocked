using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Overclocked.Api.Routing;
using Overclocked.Architecture.Tests.FakeData;
using Overclocked.Domain.BrandAggregate;
using Overclocked.Domain.CategoryAggregate;
using Overclocked.Domain.Common.StaticData;
using Overclocked.Domain.ProductAggregate;
using Overclocked.Domain.ProductAggregate.Events;
using Overclocked.Domain.TagAggregate;
using Overclocked.Infrastructure.Outbox;
using Overclocked.Infrastructure.Persistence;
using Overclocked.Integration.Tests.Shared;
using Shouldly;

namespace Overclocked.Integration.Tests.ProductTests;

[Collection(nameof(SharedTestCollection))]
public class UpdateProductTest(CustomWebApplicationFactory factory) : IAsyncLifetime
{
    private readonly HttpClient _client = factory.HttpClient;

    public async Task InitializeAsync()
    {
        await factory.ResetDatabaseAsync();

        var token = CustomWebApplicationFactory.GenerateJwtToken(
            permissions: [PermissionType.AddEditDelete.ToString()]);
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
            ProductRoutes.Update.Replace("{id:guid}", product.Id.Value.ToString()),
            form);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        using IServiceScope scope = factory.Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        Product? updatedProduct = await dbContext.Products.FindAsync(product.Id);
        updatedProduct.ShouldNotBeNull();
        updatedProduct.Name.ShouldBe("New Name");
    }

    [Fact]
    public async Task Update_Should_ReturnSuccess_When_DataIsValidAndImagesRemoved()
    {
        // Arrange
        IEnumerable<string> newImages =
        [
            "https://res.cloudinary.com/over-clocked/image1.png"
        ];

        (Guid brandId, Guid categoryId, IEnumerable<Guid> tags) = await SeedDependantEntityAsync();

        Product product = await SeedDatabaseAsync(brandId, categoryId);
        StringContent form = CreateJsonContent("New Name", brandId, categoryId, tags);
        await _client.PutAsync(ProductRoutes.Update.Replace("{id:guid}", product.Id.Value.ToString()), form);

        StringContent form2 = CreateJsonContent("New Name 2", brandId, categoryId, tags, newImages);

        // Act
        HttpResponseMessage response = await _client.PutAsync(
            ProductRoutes.Update.Replace("{id:guid}", product.Id.Value.ToString()),
            form2);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        using IServiceScope scope = factory.Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        Product? updatedProduct = await dbContext.Products.Include(x => x.Images)
            .FirstOrDefaultAsync(x => x.Id == product.Id);

        updatedProduct.ShouldNotBeNull();
        updatedProduct.Name.ShouldBe("New Name 2");
        updatedProduct.Images.Select(x => x.ImageUrl).ShouldBe(newImages);
        updatedProduct.Images.Count.ShouldBe(1);

        OutboxMessage? message = await dbContext.Set<OutboxMessage>()
            .FirstOrDefaultAsync(x => x.Type == nameof(ProductImagesRemovedEvent));

        message.ShouldNotBeNull();
        message.ProcessedOnUtc.ShouldBeNull();
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
            ProductRoutes.Update.Replace("{id:guid}", product.Id.Value.ToString()),
            form);

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

        return (brand.Id.Value, category.Id.Value, tags.Select(x => x.Id.Value));
    }

    private static StringContent CreateJsonContent(
        string name,
        Guid brandId,
        Guid categoryId,
        IEnumerable<Guid> tags,
        IEnumerable<string>? newImages = null)
    {
        IEnumerable<string> images = newImages ??
        [
            "https://res.cloudinary.com/over-clocked/image1.png",
            "https://res.cloudinary.com/over-clocked/image2.png"
        ];
        var payload = new
        {
            BrandId = brandId,
            CategoryId = categoryId,
            Name = name,
            Thumbnail = "https://res.cloudinary.com/over-clocked/image.png",
            Description = "Product Description",
            Price = 100.0m,
            StockQuantity = 10,
            Discount = 0.0m,
            Tags = tags,
            Images = images,
            Specifications = new[]
            {
                new { Name = "Name", Value = "Value" },
                new { Name = "Name 2", Value = "Value 2" }
            },
        };

        var json = JsonSerializer.Serialize(payload);
        return new StringContent(json, Encoding.UTF8, "application/json");
    }
}
