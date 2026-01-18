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
using Overclocked.Domain.TagAggregate;
using Overclocked.Domain.UserAggregate;
using Overclocked.Infrastructure.Authentication;
using Overclocked.Infrastructure.Persistence;
using Overclocked.Integration.Tests.Shared;
using Shouldly;

namespace Overclocked.Integration.Tests.ReviewTests;

public class UpdateReviewTest(IntegrationTestWebAppFactory fixture) : IAsyncLifetime
{
    private readonly HttpClient _client = fixture.HttpClient;

    public async ValueTask InitializeAsync()
    {
        await fixture.ResetDatabaseAsync();

        var token = fixture.GenerateJwtToken();
        fixture.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    [Fact]
    public async Task Update_Should_UpdateAndReturnSuccess_When_DataIsValid()
    {
        // Arrange
        (User user, Product product, Review review) = await SeedDatabaseAsync();
        StringContent form = CreateJsonContent();

        var token = fixture.GenerateJwtToken(user.Id.Value.ToString());
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        HttpResponseMessage response = await _client.PutAsync(
            ReviewRoutes
                .Update.Replace("{productId:guid}", product.Id.Value.ToString())
                .Replace("{id:guid}", review.Id.Value.ToString()),
            form
        );

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);

        using IServiceScope scope = fixture.Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        Review? reviewDb = await dbContext.Reviews.FirstOrDefaultAsync(x => x.UserId == user.Id);

        reviewDb.ShouldNotBeNull();
        reviewDb.ProductId.ShouldBe(product.Id);
        reviewDb.Comment.ShouldBe("New Comment");
        reviewDb.Rating.ShouldBe(4);
    }

    private async Task<(User user, Product product, Review review)> SeedDatabaseAsync()
    {
        User user = new UserFaker(new PasswordHasher()).Generate();
        Brand brand = new BrandFaker().Generate();
        Category category = new CategoryFaker().Generate();
        Tag tag = new TagFaker().Generate();
        Product product = new ProductFaker(brand.Id.Value, category.Id.Value, [tag.Id.Value]).Generate();
        Review review = new ReviewFaker(user.Id.Value, product.Id.Value).Generate();

        using IServiceScope scope = fixture.Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        dbContext.Brands.Add(brand);
        dbContext.Tags.Add(tag);
        dbContext.Categories.Add(category);
        dbContext.Products.Add(product);
        dbContext.Users.Add(user);
        dbContext.Reviews.Add(review);
        await dbContext.SaveChangesAsync();

        return (user, product, review);
    }

    private static StringContent CreateJsonContent()
    {
        var payload = new { Rating = 4, Comment = "New Comment" };

        var json = JsonSerializer.Serialize(payload);

        return new StringContent(json, Encoding.UTF8, "application/json");
    }
}
