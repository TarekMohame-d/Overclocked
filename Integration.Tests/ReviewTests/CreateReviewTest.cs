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
public class CreateReviewTest(CustomWebApplicationFactory factory) : IAsyncLifetime
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
    public async Task Create_Should_CreateAndReturnSuccess_When_DataIsValid()
    {
        // Arrange
        (User user, Product product) = await SeedDatabaseAsync();
        StringContent form = CreateJsonContent();

        var token = CustomWebApplicationFactory.GenerateJwtToken(user.Id.ToString());
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        HttpResponseMessage response = await _client
            .PostAsync(ReviewRoutes.Create.Replace("{productId:guid}", product.Id.ToString()), form);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Created);

        Result<ReviewCreatedResponse>? result = await response.Content
            .ReadFromJsonAsync<Result<ReviewCreatedResponse>>();

        using IServiceScope scope = factory.Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        Review? review = await dbContext.Reviews.SingleOrDefaultAsync(x => x.UserId == user.Id);
        review.ShouldNotBeNull();

        review.ProductId.ShouldBe(product.Id);

        Product? productDb = await dbContext.Products.SingleOrDefaultAsync(x => x.Id == product.Id);

        productDb.ShouldNotBeNull();
        productDb.Rating.ShouldBe(4);
        productDb.ReviewCount.ShouldBe(1);

        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.Created);
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBeNull();
    }

    [Fact]
    public async Task Create_Should_ReturnFailure_When_UserAlreadyHasReviewForProduct()
    {
        // Arrange
        (User user, Product product) = await SeedDatabaseAsync();
        StringContent form = CreateJsonContent();

        var token = CustomWebApplicationFactory.GenerateJwtToken(user.Id.ToString());
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        HttpResponseMessage response = await _client
            .PostAsync(ReviewRoutes.Create.Replace("{productId:guid}", product.Id.ToString()), form);

        HttpResponseMessage response2 = await _client
            .PostAsync(ReviewRoutes.Create.Replace("{productId:guid}", product.Id.ToString()), form);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Created);

        response2.StatusCode.ShouldBe(HttpStatusCode.Conflict);

        Result<ReviewCreatedResponse>? result = await response.Content
            .ReadFromJsonAsync<Result<ReviewCreatedResponse>>();

        Result<ReviewCreatedResponse>? result2 = await response2.Content
            .ReadFromJsonAsync<Result<ReviewCreatedResponse>>();

        using IServiceScope scope = factory.Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        Review? review = await dbContext.Reviews.SingleOrDefaultAsync(x => x.UserId == user.Id);
        review.ShouldNotBeNull();

        review.ProductId.ShouldBe(product.Id);

        Product? productDb = await dbContext.Products.SingleOrDefaultAsync(x => x.Id == product.Id);

        productDb.ShouldNotBeNull();
        productDb.Rating.ShouldBe(4);
        productDb.ReviewCount.ShouldBe(1);

        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.Created);
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBeNull();

        result2.ShouldNotBeNull();
        result2.StatusCode.ShouldBe(HttpStatusCode.Conflict);
        result2.IsSuccess.ShouldBeFalse();
        result2.Error.ShouldNotBeNull();
    }

    private async Task<(User user, Product product)> SeedDatabaseAsync()
    {
        User user = new UserFaker().Generate();
        Brand brand = new BrandFaker().Generate();
        Category category = new CategoryFaker().Generate();
        Product product = new ProductFaker().Generate();

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

        return (user, product);
    }

    private static StringContent CreateJsonContent()
    {
        var payload = new { Rating = 4, Comment = "Comment" };

        var json = JsonSerializer.Serialize(payload);

        return new StringContent(json, Encoding.UTF8, "application/json");
    }
}
