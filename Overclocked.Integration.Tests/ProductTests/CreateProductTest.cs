using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Overclocked.Api.Routing;
using Overclocked.Architecture.Tests.FakeData;
using Overclocked.Domain.BrandAggregate;
using Overclocked.Domain.CategoryAggregate;
using Overclocked.Domain.ProductAggregate;
using Overclocked.Domain.TagAggregate;
using Overclocked.Domain.UserAggregate.Enums;
using Overclocked.Infrastructure.Persistence;
using Overclocked.Integration.Tests.Shared;
using Shouldly;

namespace Overclocked.Integration.Tests.ProductTests;

[Collection(nameof(SharedTestCollection))]
public class CreateProductTest(CustomWebApplicationFactory factory) : IAsyncLifetime
{
    private readonly HttpClient _client = factory.HttpClient;

    public async Task InitializeAsync()
    {
        await factory.ResetDatabaseAsync();

        var token = CustomWebApplicationFactory.GenerateJwtToken(
            permissions: [Permission.AddEditDelete.ToString()]);
        factory.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Create_Should_CreateAndReturnSuccess_When_DataIsValid()
    {
        // Arrange
        const string Name = "AMD";

        (Guid brandId, Guid categoryId, IEnumerable<Guid> tags) = await SeedDependantEntityAsync();
        StringContent form = CreateJsonContent(Name, brandId, categoryId, tags);

        // Act
        HttpResponseMessage response = await _client.PostAsync(ProductRoutes.Create, form);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Created);

        using IServiceScope scope = factory.Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        Product? product = await dbContext.Products.SingleOrDefaultAsync(x => x.NormalizedName == Name.ToUpper());
        product.ShouldNotBeNull();
        product.Name.ShouldBe(Name);
    }

    [Fact]
    public async Task Create_Should_ReturnBadRequest_When_NameAlreadyExists()
    {
        // Arrange
        (Guid brandId, Guid categoryId, IEnumerable<Guid> tags) = await SeedDependantEntityAsync();

        Product product = await SeedDatabaseAsync(brandId, categoryId);

        StringContent form = CreateJsonContent(product.Name, brandId, categoryId, tags);

        // Act
        HttpResponseMessage response = await _client.PostAsync(ProductRoutes.Create, form);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Create_Should_ReturnForbidden_When_UserDoesNotHavePermission()
    {
        // Arrange
        const string Name = "AMD";

        (Guid brandId, Guid categoryId, IEnumerable<Guid> tags) = await SeedDependantEntityAsync();
        StringContent form = CreateJsonContent(Name, brandId, categoryId, tags);

        var token = CustomWebApplicationFactory.GenerateJwtToken();
        factory.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        HttpResponseMessage response = await _client.PostAsync(ProductRoutes.Create, form);

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

    private static StringContent CreateJsonContent(string name, Guid brandId, Guid categoryId, IEnumerable<Guid> tags)
    {
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
            Images = new List<string>
            {
                "https://res.cloudinary.com/over-clocked/image1.png",
                "https://res.cloudinary.com/over-clocked/image2.png",
            },
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
