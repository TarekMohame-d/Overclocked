using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Api.Routing;
using Application.Common.Results;
using ArchitectureTests.FakeData;
using Domain.Entities;
using Domain.StaticData;
using Infrastructure.Data;
using Integration.Tests.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Shouldly;

namespace Integration.Tests.ReviewReplyTests;

[Collection(nameof(SharedTestCollection))]
public class CreateReviewReplyTest(CustomWebApplicationFactory factory) : IAsyncLifetime
{
    private readonly HttpClient _client = factory.HttpClient;

    public async Task InitializeAsync()
    {
        await factory.ResetDatabaseAsync();
        factory.FileStorageServiceMock.ClearReceivedCalls();

        var token = CustomWebApplicationFactory.GenerateJwtToken(
            permissions: [PermissionType.ReplyToReview.ToString()]);
        factory.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Create_Should_ReturnFailure_When_UserDoesNotHavePermission()
    {
        // Arrange
        (User employee, Review review, Product product) = await SeedDatabaseAsync(true);
        StringContent form = CreateJsonContent();

        var token = CustomWebApplicationFactory.GenerateJwtToken(employee.Id.ToString());
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        HttpResponseMessage response = await _client
            .PostAsync(ReviewReplyRoutes.Create.Replace("{productId:guid}", product.Id.ToString()).Replace("{reviewId:guid}", review.Id.ToString()), form);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Create_Should_CreateAndReturnSuccess_When_DataIsValid()
    {
        // Arrange
        (User employee, Review review, Product product) = await SeedDatabaseAsync();
        StringContent form = CreateJsonContent();

        var token = CustomWebApplicationFactory.GenerateJwtToken(employee.Id.ToString(),
            permissions: [PermissionType.ReplyToReview.ToString()]);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        HttpResponseMessage response = await _client
            .PostAsync(ReviewReplyRoutes.Create.Replace("{productId:guid}", product.Id.ToString()).Replace("{reviewId:guid}", review.Id.ToString()), form);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Created);

        Result? result = await response.Content.ReadFromJsonAsync<Result>();

        using IServiceScope scope = factory.Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        ReviewReply? reviewReplyDb = await dbContext.ReviewReplies.SingleOrDefaultAsync(x => x.ReviewId == review.Id);
        reviewReplyDb.ShouldNotBeNull();
        reviewReplyDb.EmployeeId.ShouldBe(employee.Id);
        reviewReplyDb.ReviewId.ShouldBe(review.Id);

        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.Created);
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBeNull();
    }

    [Fact]
    public async Task Create_Should_ReturnFailure_When_ReviewAlreadyHasReply()
    {
        // Arrange
        (User employee, Review review, Product product) = await SeedDatabaseAsync(true);
        StringContent form = CreateJsonContent();

        var token = CustomWebApplicationFactory.GenerateJwtToken(employee.Id.ToString(),
            permissions: [PermissionType.ReplyToReview.ToString()]);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        HttpResponseMessage response = await _client
            .PostAsync(ReviewReplyRoutes.Create.Replace("{productId:guid}", product.Id.ToString()).Replace("{reviewId:guid}", review.Id.ToString()), form);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);

        Result? result = await response.Content.ReadFromJsonAsync<Result>();

        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.Conflict);
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBeNull();
    }

    private async Task<(User employee, Review review, Product product)> SeedDatabaseAsync(bool alreadyReplied = false)
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

        product.CategoryId = category.Id;
        product.BrandId = brand.Id;
        product.Brand = brand;
        product.Category = category;

        user.Reviews.Add(review);

        var role = new Role { Name = "Customer", Id = 4 };

        user.RoleType = RoleType.Customer;
        user.Role = role;

        User employee = new UserFaker().Generate();
        var roleEmployee = new Role { Name = "Admin", Id = 2 };

        employee.RoleType = RoleType.Admin;
        employee.Role = roleEmployee;
        if(alreadyReplied)
        {
            var reviewReply = new ReviewReply
            {
                EmployeeId = employee.Id,
                ReviewId = review.Id,
                Reply = "Reply"
            };

            review.ReviewReply = reviewReply;
        }

        using IServiceScope scope = factory.Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        dbContext.Products.Add(product);
        dbContext.Users.Add(user);
        dbContext.Users.Add(employee);
        await dbContext.SaveChangesAsync();

        return (employee, review, product);
    }

    private static StringContent CreateJsonContent()
    {
        var payload = new { Reply = "Reply" };

        var json = JsonSerializer.Serialize(payload);

        return new StringContent(json, Encoding.UTF8, "application/json");
    }
}
