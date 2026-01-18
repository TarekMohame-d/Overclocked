using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Overclocked.Api.Routing;
using Overclocked.Application.Features.ReviewUseCases.DTOs.Responses;
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

public class GetRatingBreakDownTest(IntegrationTestWebAppFactory fixture) : IAsyncLifetime
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
    public async Task GetRatingBreakDown_Should_ReturnSuccess_When_DataIsValid()
    {
        // Arrange
        (User user, Product product, Review review) = await SeedDatabaseAsync();

        // Act
        HttpResponseMessage response = await _client.GetAsync(
            ReviewRoutes.GetReviewsRatingBreakdown.Replace("{productId:guid}", product.Id.Value.ToString())
        );

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        RatingBreakdownResponse? result = await response.Content.ReadFromJsonAsync<RatingBreakdownResponse>();

        result.ShouldNotBeNull();
        result.Ratings.ShouldNotBeNull();
        result.Ratings.Count.ShouldBe(5);
        result.Ratings[review.Rating].ShouldBe(1);
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
}
