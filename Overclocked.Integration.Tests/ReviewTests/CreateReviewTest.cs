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
using Overclocked.Domain.ReviewAggregate;
using Overclocked.Domain.UserAggregate;
using Overclocked.Infrastructure.Authentication;
using Overclocked.Infrastructure.Persistence;
using Overclocked.Integration.Tests.Shared;
using Shouldly;

namespace Overclocked.Integration.Tests.ReviewTests;

[Collection(nameof(SharedTestCollection))]
public class CreateReviewTest(CustomWebApplicationFactory factory) : IAsyncLifetime
{
    private readonly HttpClient _client = factory.HttpClient;

    public async Task InitializeAsync()
    {
        await factory.ResetDatabaseAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Create_Should_CreateAndReturnSuccess_When_DataIsValid()
    {
        // Arrange
        (User user, Product product) = await SeedDatabaseAsync();
        StringContent form = CreateJsonContent();

        var token = CustomWebApplicationFactory.GenerateJwtToken(user.Id.Value.ToString());
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        HttpResponseMessage response = await _client
            .PostAsync(ReviewRoutes.Create.Replace("{productId:guid}", product.Id.Value.ToString()), form);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Created);

        using IServiceScope scope = factory.Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        Review? review = await dbContext.Reviews.FirstOrDefaultAsync(x => x.UserId == user.Id);

        review.ShouldNotBeNull();
        review.ProductId.ShouldBe(product.Id);
    }

    [Fact]
    public async Task Create_Should_ReturnFailure_When_UserAlreadyHasReviewForProduct()
    {
        // Arrange
        (User user, Product product) = await SeedDatabaseAsync();
        StringContent form = CreateJsonContent();

        var token = CustomWebApplicationFactory.GenerateJwtToken(user.Id.Value.ToString());
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        HttpResponseMessage response = await _client
            .PostAsync(ReviewRoutes.Create.Replace("{productId:guid}", product.Id.Value.ToString()), form);

        HttpResponseMessage response2 = await _client
            .PostAsync(ReviewRoutes.Create.Replace("{productId:guid}", product.Id.Value.ToString()), form);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Created);

        response2.StatusCode.ShouldBe(HttpStatusCode.Conflict);

        using IServiceScope scope = factory.Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        Review? review = await dbContext.Reviews.SingleOrDefaultAsync(x => x.UserId == user.Id);

        review.ShouldNotBeNull();
        review.ProductId.ShouldBe(product.Id);
    }

    private async Task<(User user, Product product)> SeedDatabaseAsync()
    {
        User user = new UserFaker(new PasswordHasher()).Generate();
        Brand brand = new BrandFaker().Generate();
        Category category = new CategoryFaker().Generate();
        Product product = new ProductFaker(brand.Id.Value, category.Id.Value).Generate();

        using IServiceScope scope = factory.Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        dbContext.Brands.Add(brand);
        dbContext.Categories.Add(category);
        dbContext.Products.Add(product);
        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();

        return (user, product);
    }

    private static StringContent CreateJsonContent()
    {
        var payload = new { Rating = 4, Comment = "Comment" };

        var json = JsonSerializer.Serialize(payload);

        return new StringContent(json, Encoding.UTF8, "application/json");
    }
}
