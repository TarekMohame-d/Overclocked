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
using Shouldly;

namespace Integration.Tests.WishlistTests;

[Collection(nameof(SharedTestCollection))]
public class AddWishlistItemTest(CustomWebApplicationFactory factory) : IAsyncLifetime
{
    private readonly HttpClient _client = factory.HttpClient;

    public async Task InitializeAsync() => await factory.ResetDatabaseAsync();

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task AddWishlistItem_Should_ReturnSuccess_When_DataIsValid()
    {
        // Arrange
        (User user, Product product, Wishlist wishlist) = await SeedDatabaseAsync();

        var token = CustomWebApplicationFactory.GenerateJwtToken(user.Id.ToString());
        factory.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        StringContent form = CreateJsonContent(product.Id);

        // Act
        HttpResponseMessage response = await _client.PostAsync(WishlistRoutes.AddWishlistItem, form);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        Result? result = await response.Content.ReadFromJsonAsync<Result>();

        using IServiceScope scope = factory.Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        WishlistItem? wishlistItem = await dbContext.WishlistItems
            .SingleOrDefaultAsync(x => x.WishlistId == wishlist.Id && x.ProductId == product.Id);

        wishlistItem.ShouldNotBeNull();
        wishlistItem.ProductId.ShouldBe(product.Id);

        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBeNull();
    }

    [Fact]
    public async Task AddWishlistItem_Should_DoNothing_When_WishlistItemAlreadyExists()
    {
        // Arrange
        (User user, Product product, Wishlist wishlist) = await SeedDatabaseAsync();

        var token = CustomWebApplicationFactory.GenerateJwtToken(user.Id.ToString());
        factory.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        StringContent form = CreateJsonContent(product.Id);

        // Act
        HttpResponseMessage response = await _client.PostAsync(WishlistRoutes.AddWishlistItem, form);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        Result? result = await response.Content.ReadFromJsonAsync<Result>();

        using IServiceScope scope = factory.Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        IEnumerable<WishlistItem> wishlistItem = await dbContext.WishlistItems.ToListAsync();

        wishlistItem.ShouldNotBeNull();
        wishlistItem.Count().ShouldBe(1);

        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBeNull();
    }

    private async Task<(User user, Product product, Wishlist wishlist)> SeedDatabaseAsync()
    {
        User user = new UserFaker().Generate();
        Brand brand = new BrandFaker().Generate();
        Category category = new CategoryFaker().Generate();
        Product product = new ProductFaker().Generate();
        var wishlist = new Wishlist { UserId = user.Id };

        var wishlistItem = new WishlistItem { ProductId = product.Id, WishlistId = wishlist.Id };

        var role = new Role { Name = "Customer", Id = 4 };

        user.RoleType = RoleType.Customer;

        product.CategoryId = category.Id;
        product.BrandId = brand.Id;
        product.StockQuantity = 10;

        using IServiceScope scope = factory.Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        dbContext.Brands.Add(brand);
        dbContext.Categories.Add(category);
        dbContext.Products.Add(product);
        dbContext.Roles.Add(role);
        dbContext.Users.Add(user);
        dbContext.Wishlists.Add(wishlist);
        dbContext.WishlistItems.Add(wishlistItem);
        await dbContext.SaveChangesAsync();

        return (user, product, wishlist);
    }

    private static StringContent CreateJsonContent(Guid productId)
    {
        var payload = new { ProductId = productId };

        var json = JsonSerializer.Serialize(payload);

        return new StringContent(json, Encoding.UTF8, "application/json");
    }
}
