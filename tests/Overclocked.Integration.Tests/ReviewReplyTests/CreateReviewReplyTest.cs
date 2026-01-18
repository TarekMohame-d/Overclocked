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
using Overclocked.Domain.UserAggregate.Enums;
using Overclocked.Infrastructure.Authentication;
using Overclocked.Infrastructure.Persistence;
using Overclocked.Integration.Tests.Shared;
using Shouldly;

namespace Overclocked.Integration.Tests.ReviewReplyTests;

[Collection(nameof(IntegrationTestCollection))]
public class CreateReviewReplyTest(IntegrationTestWebAppFactory factory) : IAsyncLifetime
{
    private readonly HttpClient _client = factory.HttpClient;

    public async Task InitializeAsync() => await factory.ResetDatabaseAsync();

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Create_Should_CreateAndReturnSuccess_When_DataIsValid()
    {
        // Arrange
        (User user, User admin, Product product, Review review) = await SeedDatabaseAsync();
        StringContent form = CreateJsonContent();

        var token = factory.GenerateJwtToken(userId: admin.Id.Value.ToString(), role: "Admin");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        HttpResponseMessage response = await _client.PostAsync(
            ReviewReplyRoutes
                .Create.Replace("{productId:guid}", product.Id.Value.ToString())
                .Replace("{reviewId:guid}", review.Id.Value.ToString()),
            form
        );

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Created);

        using IServiceScope scope = factory.Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        Review? reviewDb = await dbContext.Reviews.FirstOrDefaultAsync(x => x.UserId == user.Id);

        reviewDb.ShouldNotBeNull();
        reviewDb.ProductId.ShouldBe(product.Id);
        reviewDb.ReviewReply.ShouldNotBeNull();
        reviewDb.ReviewReply.Reply.ShouldBe("Reply");
        reviewDb.ReviewReply.EmployeeId.ShouldBe(admin.Id);
    }

    [Fact]
    public async Task Create_Should_ReturnFailure_When_ReviewAlreadyReply()
    {
        // Arrange
        (User user, User admin, Product product, Review review) = await SeedDatabaseAsync();
        StringContent form1 = CreateJsonContent("reply");
        StringContent form2 = CreateJsonContent("new reply");

        var token = factory.GenerateJwtToken(userId: admin.Id.Value.ToString(), role: "Admin");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        HttpResponseMessage response = await _client.PostAsync(
            ReviewReplyRoutes
                .Create.Replace("{productId:guid}", product.Id.Value.ToString())
                .Replace("{reviewId:guid}", review.Id.Value.ToString()),
            form1
        );

        HttpResponseMessage response2 = await _client.PostAsync(
            ReviewReplyRoutes
                .Create.Replace("{productId:guid}", product.Id.Value.ToString())
                .Replace("{reviewId:guid}", review.Id.Value.ToString()),
            form2
        );

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Created);

        response2.StatusCode.ShouldBe(HttpStatusCode.Conflict);

        using IServiceScope scope = factory.Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        Review? reviewDb = await dbContext.Reviews.SingleOrDefaultAsync(x => x.UserId == user.Id);

        reviewDb.ShouldNotBeNull();
        review.ProductId.ShouldBe(product.Id);
        reviewDb.ReviewReply.ShouldNotBeNull();
        reviewDb.ReviewReply.Reply.ShouldBe("reply");
        reviewDb.ReviewReply.EmployeeId.ShouldBe(admin.Id);
    }

    private async Task<(User user, User admin, Product product, Review review)> SeedDatabaseAsync()
    {
        User user = new UserFaker(new PasswordHasher()).Generate();
        Brand brand = new BrandFaker().Generate();
        Category category = new CategoryFaker().Generate();
        Tag tag = new TagFaker().Generate();
        Product product = new ProductFaker(brand.Id.Value, category.Id.Value, [tag.Id.Value]).Generate();
        Review review = new ReviewFaker(user.Id.Value, product.Id.Value).Generate();

        User admin = new UserFaker(new PasswordHasher()).Generate();
        admin.ChangeRole(Role.Admin);

        using IServiceScope scope = factory.Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        dbContext.Brands.Add(brand);
        dbContext.Tags.Add(tag);
        dbContext.Categories.Add(category);
        dbContext.Products.Add(product);
        dbContext.Users.Add(user);
        dbContext.Users.Add(admin);
        dbContext.Reviews.Add(review);
        await dbContext.SaveChangesAsync();

        return (user, admin, product, review);
    }

    private static StringContent CreateJsonContent(string reply = "Reply")
    {
        var payload = new { Reply = reply };

        var json = JsonSerializer.Serialize(payload);

        return new StringContent(json, Encoding.UTF8, "application/json");
    }
}
