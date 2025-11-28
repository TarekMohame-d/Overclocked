using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Api.Routing;
using Application.Common.Results;
using Application.Services.Review.DTOs.Response;
using ArchitectureTests.FakeData;
using Domain.Entities;
using Domain.StaticData;
using Infrastructure.Data;
using Integration.Tests.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Shouldly;

namespace Integration.Tests.ReviewTests;

[Collection(nameof(SharedTestCollection))]
public class UpdateReviewTest(CustomWebApplicationFactory factory) : IAsyncLifetime
{
    private readonly HttpClient _client = factory.HttpClient;

    public async Task InitializeAsync()
    {
        await factory.ResetDatabaseAsync();
        factory.FileStorageServiceMock.ClearReceivedCalls();

        var token = CustomWebApplicationFactory.GenerateJwtToken();
        factory.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Update_Should_UpdateReviewAndUpdateProduct_When_DataIsValid()
    {
        // Arrange
        (User user, Product product, Review review, var rating, var reviewCount) = await SeedDatabaseAsync();

        StringContent form = CreateJsonContent();

        var token = CustomWebApplicationFactory.GenerateJwtToken(user.Id.ToString());
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        HttpResponseMessage response = await _client
            .PutAsync(ReviewRoutes.Update.Replace("{productId:guid}", product.Id.ToString()).Replace("{id:guid}", review.Id.ToString()), form);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        Result<ReviewUpdatedResponse>? result = await response.Content
            .ReadFromJsonAsync<Result<ReviewUpdatedResponse>>();

        using IServiceScope scope = factory.Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        Review? reviewDb = await dbContext.Reviews
            .SingleOrDefaultAsync(x => x.UserId == user.Id && x.ProductId == product.Id);

        reviewDb.ShouldNotBeNull();
        reviewDb.Rating.ShouldBe(2);
        reviewDb.Comment.ShouldBe("new comment");

        Product? productDb = await dbContext.Products.SingleOrDefaultAsync(x => x.Id == product.Id);

        productDb.ShouldNotBeNull();

        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBeNull();
    }

    private async Task<(User user, Product product, Review review, double rating, int reviewCount)> SeedDatabaseAsync()
    {
        User user = new UserFaker().Generate();
        Brand brand = new BrandFaker().Generate();
        Category category = new CategoryFaker().Generate();
        Product product = new ProductFaker().Generate();
        Review review = new ReviewFaker().Generate();

        review.ProductId = product.Id;
        review.UserId = user.Id;
        review.Rating = 5;
        review.Comment = "Good Product";

        product.Reviews.Add(review);

        product.CalculateRating(review.Rating);

        product.CategoryId = category.Id;
        product.BrandId = brand.Id;
        product.Brand = brand;
        product.Category = category;

        var role = new Role { Name = "Customer", Id = 4 };

        user.RoleType = RoleType.Customer;
        user.Role = role;

        using IServiceScope scope = factory.Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        dbContext.Products.Add(product);
        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();

        return (user, product, review, product.Rating, product.ReviewCount);
    }

    private static StringContent CreateJsonContent()
    {
        var payload = new { Rating = 2, Comment = "new comment" };

        var json = JsonSerializer.Serialize(payload);

        return new StringContent(json, Encoding.UTF8, "application/json");
    }
}
