using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
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
using Role = Domain.Entities.Role;

namespace Integration.Tests.WishlistTests;

[Collection(nameof(SharedTestCollection))]
public class DeleteWishlistItemTest(CustomWebApplicationFactory factory) : IAsyncLifetime
{
    private readonly HttpClient _client = factory.HttpClient;

    public async Task InitializeAsync() => await factory.ResetDatabaseAsync();

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task DeleteWishlistItem_Should_ReturnSuccess_When_DataIsValid()
    {
        // Arrange
        (User user, Product product, Product product2, Wishlist wishlist) = await SeedDatabaseAsync();

        var token = CustomWebApplicationFactory.GenerateJwtToken(user.Id.ToString());
        factory.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        HttpResponseMessage response = await _client
            .DeleteAsync(WishlistRoutes.DeleteWishlistItem.Replace("{id:guid}", wishlist.WishlistItems.First().Id.ToString()));

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        Result? result = await response.Content.ReadFromJsonAsync<Result>();

        using IServiceScope scope = factory.Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        Wishlist? wishlistDb = await dbContext.Wishlists.Include(x => x.WishlistItems)
            .SingleOrDefaultAsync(x => x.UserId == user.Id);

        wishlistDb.ShouldNotBeNull();
        wishlistDb.WishlistItems.Count.ShouldBe(1);
        wishlistDb.WishlistItems.Any(ci => ci.ProductId == product2.Id).ShouldBeTrue();

        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBeNull();
    }

    private async Task<(User user, Product product, Product product2, Wishlist wishlist)> SeedDatabaseAsync()
    {
        User user = new UserFaker().Generate();
        Brand brand = new BrandFaker().Generate();
        Category category = new CategoryFaker().Generate();
        Product product = new ProductFaker().Generate();
        Product product2 = new ProductFaker().Generate();
        var wishlist = new Wishlist { UserId = user.Id };

        wishlist.WishlistItems.Add(new WishlistItem
        {
            WishlistId = wishlist.Id,
            ProductId = product.Id
        });

        wishlist.WishlistItems.Add(new WishlistItem
        {
            WishlistId = wishlist.Id,
            ProductId = product2.Id
        });

        var role = new Role { Name = "Customer", Id = 4 };

        user.RoleType = RoleType.Customer;

        product.CategoryId = category.Id;
        product.BrandId = brand.Id;
        product.StockQuantity = 10;

        product2.CategoryId = category.Id;
        product2.BrandId = brand.Id;
        product2.StockQuantity = 20;

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

        return (user, product, product2, wishlist);
    }
}
