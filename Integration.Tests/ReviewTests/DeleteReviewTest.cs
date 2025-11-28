using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
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
public class DeleteReviewTest(CustomWebApplicationFactory factory) : IAsyncLifetime
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
    public async Task Delete_Should_DeleteReviewAndUpdateProduct_When_DataIsValid()
    {
        // Arrange
        (User user, Product product, Review review, var rating, var reviewCount) = await SeedDatabaseAsync();

        var token = CustomWebApplicationFactory.GenerateJwtToken(user.Id.ToString());
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        HttpResponseMessage response = await _client
            .DeleteAsync(ReviewRoutes.Delete.Replace("{productId:guid}", product.Id.ToString()).Replace("{id:guid}", review.Id.ToString()));

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        Result<ReviewDeletedResponse>? result = await response.Content
            .ReadFromJsonAsync<Result<ReviewDeletedResponse>>();

        using IServiceScope scope = factory.Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        Review? reviewDb = await dbContext.Reviews
            .SingleOrDefaultAsync(x => x.UserId == user.Id && x.ProductId == product.Id);

        reviewDb.ShouldBeNull();

        Product? productDb = await dbContext.Products.SingleOrDefaultAsync(x => x.Id == product.Id);

        productDb.ShouldNotBeNull();
        productDb.Rating.ShouldBe(((rating * reviewCount) - review.Rating) / (reviewCount - 1));
        productDb.ReviewCount.ShouldBe(reviewCount - 1);

        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBeNull();
    }

    private async Task<(User user, Product product, Review review, double rating, int reviewCount)> SeedDatabaseAsync()
    {
        User user = new UserFaker().Generate();
        User user2 = new UserFaker().Generate();
        Brand brand = new BrandFaker().Generate();
        Category category = new CategoryFaker().Generate();
        Product product = new ProductFaker().Generate();
        Review review = new ReviewFaker().Generate();
        Review review2 = new ReviewFaker().Generate();

        review.ProductId = product.Id;
        review.UserId = user.Id;
        review.Rating = 5;
        review.Comment = "Good Product";

        review2.ProductId = product.Id;
        review2.UserId = user2.Id;
        review2.Rating = 4;
        review2.Comment = "Good Product";

        product.Reviews.Add(review);
        product.Reviews.Add(review2);

        product.CalculateRating(review.Rating);
        product.CalculateRating(review2.Rating);

        product.CategoryId = category.Id;
        product.BrandId = brand.Id;
        product.Brand = brand;
        product.Category = category;

        var role = new Role { Name = "Customer", Id = 4 };

        user.RoleType = RoleType.Customer;
        user.Role = role;

        user2.RoleType = RoleType.Customer;
        user2.Role = role;

        using IServiceScope scope = factory.Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        dbContext.Products.Add(product);
        dbContext.Users.Add(user);
        dbContext.Users.Add(user2);
        await dbContext.SaveChangesAsync();

        return (user, product, review, product.Rating, product.ReviewCount);
    }
}
