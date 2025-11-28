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
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Shouldly;

namespace Integration.Tests.ReviewTests;

[Collection(nameof(SharedTestCollection))]
public class GetPagedReviewsTest(CustomWebApplicationFactory factory) : IAsyncLifetime
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
    public async Task GetPagedReviews_Should_ReturnData_When_ThereIsReviewsForProduct()
    {
        // Arrange
        Product product = await SeedDatabaseAsync();

        // Act
        var url = $"{ReviewRoutes.GetAll.Replace("{productId:guid}", product.Id.ToString())}?Page=1&PageSize=10&SortBy=createdAt&Direction=desc";
        HttpResponseMessage response = await _client.GetAsync(url);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        Result<PagedResult<ReviewResponse>>? result = await response.Content
            .ReadFromJsonAsync<Result<PagedResult<ReviewResponse>>>();

        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.Data.ShouldNotBeNull();
        result.Data.Items.ShouldNotBeEmpty();
        result.Data.Items.Count.ShouldBe(2);
    }

    [Fact]
    public async Task GetPagedReviews_Should_ReturnEmptyList_When_ThereIsNoData()
    {
        // Arrange

        // Act
        var url = $"{ReviewRoutes.GetAll.Replace("{productId:guid}", Guid.NewGuid().ToString())}?Page=1&PageSize=10&SortBy=createdAt&Direction=desc";
        HttpResponseMessage response = await _client.GetAsync(url);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        Result<PagedResult<ReviewResponse>>? result = await response.Content
            .ReadFromJsonAsync<Result<PagedResult<ReviewResponse>>>();

        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.Data.ShouldNotBeNull();
        result.Data.Items.ShouldBeEmpty();
        result.Data.Items.Count.ShouldBe(0);
    }

    private async Task<Product> SeedDatabaseAsync()
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

        return product;
    }
}
