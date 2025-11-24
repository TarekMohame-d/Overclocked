using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Api.Routing;
using Application.Common.Results;
using Application.Services.Wishlist.DTOs.Response;
using ArchitectureTests.FakeData;
using Domain.Entities;
using Domain.StaticData;
using Infrastructure.Data;
using Integration.Tests.Shared;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Integration.Tests.WishlistTests;

[Collection(nameof(SharedTestCollection))]
public class GetWishlistItemsTest(CustomWebApplicationFactory factory) : IAsyncLifetime
{
    private readonly HttpClient _client = factory.HttpClient;

    public async Task InitializeAsync() => await factory.ResetDatabaseAsync();

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task GetWishlistItems_Should_ReturnSuccess_When_DataIsValid()
    {
        // Arrange
        User user = await SeedDatabaseAsync(10);

        var token = CustomWebApplicationFactory.GenerateJwtToken(user.Id.ToString());
        factory.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        HttpResponseMessage response = await _client.GetAsync(WishlistRoutes.GetWishlistItems);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        Result<IEnumerable<WishlistItemResponse>>? result = await response.Content.
            ReadFromJsonAsync<Result<IEnumerable<WishlistItemResponse>>>();

        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.Data.ShouldNotBeNull();
        result.Data.Count().ShouldBe(2);
    }

    private async Task<User> SeedDatabaseAsync(int quantity = 10)
    {
        User user = new UserFaker().Generate();
        Brand brand = new BrandFaker().Generate();
        Category category = new CategoryFaker().Generate();
        Product product = new ProductFaker().Generate();
        Product product2 = new ProductFaker().Generate();
        var wishlist = new Wishlist { User = user, UserId = user.Id };

        wishlist.WishlistItems.Add(new WishlistItem
        {
            WishlistId = wishlist.Id,
            ProductId = product.Id,
        });

        wishlist.WishlistItems.Add(new WishlistItem
        {
            WishlistId = wishlist.Id,
            ProductId = product2.Id,
        });

        var role = new Role { Name = "Customer", Id = 4 };

        user.RoleType = RoleType.Customer;

        product.CategoryId = category.Id;
        product.BrandId = brand.Id;
        product.StockQuantity = quantity;

        product2.CategoryId = category.Id;
        product2.BrandId = brand.Id;
        product2.StockQuantity = quantity;

        using IServiceScope scope = factory.Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        dbContext.Brands.Add(brand);
        dbContext.Categories.Add(category);
        dbContext.Products.Add(product);
        dbContext.Products.Add(product2);
        dbContext.Roles.Add(role);
        dbContext.Users.Add(user);
        dbContext.Wishlists.Add(wishlist);
        await dbContext.SaveChangesAsync();

        return user;
    }
}
