using System.Net;
using System.Net.Http.Headers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Overclocked.Api.Routing;
using Overclocked.Architecture.Tests.FakeData;
using Overclocked.Domain.BrandAggregate;
using Overclocked.Domain.CategoryAggregate;
using Overclocked.Domain.ProductAggregate;
using Overclocked.Domain.ReviewAggregate;
using Overclocked.Domain.ReviewAggregate.Entities;
using Overclocked.Domain.TagAggregate;
using Overclocked.Domain.UserAggregate;
using Overclocked.Domain.UserAggregate.Enums;
using Overclocked.Infrastructure.Authentication;
using Overclocked.Infrastructure.Persistence;
using Overclocked.Integration.Tests.Shared;
using Shouldly;

namespace Overclocked.Integration.Tests.ReviewReplyTests;

public class DeleteReviewReplyTest(IntegrationTestWebAppFactory fixture) : IAsyncLifetime
{
    private readonly HttpClient _client = fixture.HttpClient;

    public async ValueTask InitializeAsync() => await fixture.ResetDatabaseAsync();

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    [Fact]
    public async Task Delete_Should_DeleteAndReturnSuccess_When_DataIsValid()
    {
        // Arrange
        (User user, User admin, User admin2, Product product, Review review) = await SeedDatabaseAsync();

        var token = fixture.GenerateJwtToken(userId: admin.Id.Value.ToString(), role: "Admin");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        HttpResponseMessage response = await _client.DeleteAsync(
            ReviewReplyRoutes
                .Delete.Replace("{productId:guid}", product.Id.Value.ToString())
                .Replace("{reviewId:guid}", review.Id.Value.ToString())
                .Replace("{replyId:guid}", review.ReviewReply!.Id.Value.ToString())
        );

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);

        using IServiceScope scope = fixture.Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        Review? reviewDb = await dbContext.Reviews.FirstOrDefaultAsync(x => x.UserId == user.Id);

        reviewDb.ShouldNotBeNull();
        reviewDb.ProductId.ShouldBe(product.Id);
        reviewDb.ReviewReply.ShouldBeNull();
    }

    [Fact]
    public async Task Update_Should_ReturnFailure_When_Unauthorized()
    {
        // Arrange
        (User user, User admin, User admin2, Product product, Review review) = await SeedDatabaseAsync();

        var token = fixture.GenerateJwtToken(userId: admin2.Id.Value.ToString(), role: "Admin");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        HttpResponseMessage response = await _client.DeleteAsync(
            ReviewReplyRoutes
                .Delete.Replace("{productId:guid}", product.Id.Value.ToString())
                .Replace("{reviewId:guid}", review.Id.Value.ToString())
                .Replace("{replyId:guid}", review.ReviewReply!.Id.Value.ToString())
        );

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);

        using IServiceScope scope = fixture.Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        Review? reviewDb = await dbContext.Reviews.SingleOrDefaultAsync(x => x.UserId == user.Id);

        reviewDb.ShouldNotBeNull();
        review.ProductId.ShouldBe(product.Id);
        reviewDb.ReviewReply.ShouldNotBeNull();
        reviewDb.ReviewReply.Reply.ShouldBe("Reply");
        reviewDb.ReviewReply.EmployeeId.ShouldBe(admin.Id);
    }

    private async Task<(User user, User admin, User admin2, Product product, Review review)> SeedDatabaseAsync()
    {
        User user = new UserFaker(new PasswordHasher()).Generate();
        Brand brand = new BrandFaker().Generate();
        Category category = new CategoryFaker().Generate();
        Tag tag = new TagFaker().Generate();
        Product product = new ProductFaker(brand.Id.Value, category.Id.Value, [tag.Id.Value]).Generate();
        Review review = new ReviewFaker(user.Id.Value, product.Id.Value).Generate();

        User admin = new UserFaker(new PasswordHasher()).Generate();
        admin.ChangeRole(Role.Admin);

        review.AddReviewReply(ReviewReply.Create(admin.Id, "Reply").Value);

        User admin2 = new UserFaker(new PasswordHasher()).Generate();
        admin2.ChangeRole(Role.Admin);

        using IServiceScope scope = fixture.Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        dbContext.Brands.Add(brand);
        dbContext.Tags.Add(tag);
        dbContext.Categories.Add(category);
        dbContext.Products.Add(product);
        dbContext.Users.Add(user);
        dbContext.Users.Add(admin);
        dbContext.Users.Add(admin2);
        dbContext.Reviews.Add(review);
        await dbContext.SaveChangesAsync();

        return (user, admin, admin2, product, review);
    }
}
