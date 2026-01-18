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
using Overclocked.SharedKernel;
using Shouldly;

namespace Overclocked.Integration.Tests.ReviewTests;

[Collection(nameof(IntegrationTestCollection))]
public class GetPagedReviewsTest(IntegrationTestWebAppFactory factory) : IAsyncLifetime
{
    private readonly HttpClient _client = factory.HttpClient;

    public async Task InitializeAsync()
    {
        await factory.ResetDatabaseAsync();

        var token = factory.GenerateJwtToken();
        factory.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task GetPagedReviews_Should_ReturnSuccess_When_DataIsValid()
    {
        // Arrange
        (User user, Product product, Review review) = await SeedDatabaseAsync();

        // Act
        HttpResponseMessage response = await _client.GetAsync(
            ReviewRoutes.GetPaged.Replace("{productId:guid}", product.Id.Value.ToString())
        );

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        PagedResult<ReviewResponse>? result = await response.Content.ReadFromJsonAsync<PagedResult<ReviewResponse>>();

        result.ShouldNotBeNull();
        result.Items.Count().ShouldBe(1);
        result.Items.First().Rating.ShouldBe(review.Rating);
        result.Items.First().Comment.ShouldBe(review.Comment);
    }

    private async Task<(User user, Product product, Review review)> SeedDatabaseAsync()
    {
        User user = new UserFaker(new PasswordHasher()).Generate();
        Brand brand = new BrandFaker().Generate();
        Category category = new CategoryFaker().Generate();
        Tag tag = new TagFaker().Generate();
        Product product = new ProductFaker(brand.Id.Value, category.Id.Value, [tag.Id.Value]).Generate();
        Review review = new ReviewFaker(user.Id.Value, product.Id.Value).Generate();

        using IServiceScope scope = factory.Services.CreateScope();
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
