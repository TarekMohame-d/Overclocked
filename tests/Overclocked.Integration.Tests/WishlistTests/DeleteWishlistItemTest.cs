using System.Net;
using System.Net.Http.Headers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Overclocked.Api.Routing;
using Overclocked.Architecture.Tests.FakeData;
using Overclocked.Domain.BrandAggregate;
using Overclocked.Domain.CategoryAggregate;
using Overclocked.Domain.ProductAggregate;
using Overclocked.Domain.ProductAggregate.ValueObjects;
using Overclocked.Domain.TagAggregate;
using Overclocked.Domain.UserAggregate;
using Overclocked.Domain.WishlistAggregate;
using Overclocked.Infrastructure.Authentication;
using Overclocked.Infrastructure.Persistence;
using Overclocked.Integration.Tests.Shared;
using Shouldly;

namespace Overclocked.Integration.Tests.WishlistTests;

public class DeleteWishlistItemTest(ApiTestFixture fixture) : IAsyncLifetime
{
    private readonly HttpClient _client = fixture.HttpClient;

    public async ValueTask InitializeAsync()
    {
        await fixture.ResetDatabaseAsync();

        var token = fixture.GenerateJwtToken();
        fixture.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    [Fact]
    public async Task DeleteWishlistItem_Should_ReturnForbidden_When_UserDoesNotHaveRole()
    {
        // Arrange
        var token = fixture.GenerateJwtToken(role: "Admin");
        fixture.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        HttpResponseMessage response = await _client.DeleteAsync(
            WishlistRoutes.DeleteWishlistItem.Replace("{id:guid}", Guid.NewGuid().ToString())
        );

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task DeleteWishlistItem_Should_ReturnSuccess_When_DataIsValid()
    {
        // Arrange
        (User user, ProductId productId) = await SeedDatabaseAsync();

        var token = fixture.GenerateJwtToken(userId: user.Id.Value.ToString());
        fixture.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        HttpResponseMessage response = await _client.DeleteAsync(
            WishlistRoutes.DeleteWishlistItem.Replace("{id:guid}", productId.Value.ToString())
        );

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        using IServiceScope scope = fixture.Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        Wishlist? wishlist = await dbContext.Wishlists.FirstOrDefaultAsync(x => x.UserId == user.Id);
        wishlist.ShouldNotBeNull();
        wishlist.WishlistItems.ShouldNotBeEmpty();
        wishlist.WishlistItems.Count.ShouldBe(2);
    }

    private async Task<(User user, ProductId productId)> SeedDatabaseAsync()
    {
        Brand brand = new BrandFaker().Generate();
        Category category = new CategoryFaker().Generate();
        Tag tag = new TagFaker().Generate();
        List<Product> products = new ProductFaker(brand.Id.Value, category.Id.Value, [tag.Id.Value]).Generate(5);
        User user = new UserFaker(new PasswordHasher()).Generate();
        var wishlist = Wishlist.Create(user.Id);
        wishlist.AddWishlistItem(products[0].Id);
        wishlist.AddWishlistItem(products[1].Id);
        wishlist.AddWishlistItem(products[2].Id);

        using IServiceScope scope = fixture.Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        dbContext.Brands.Add(brand);
        dbContext.Tags.Add(tag);
        dbContext.Categories.Add(category);
        dbContext.Products.AddRange(products);
        dbContext.Users.Add(user);
        dbContext.Wishlists.Add(wishlist);

        await dbContext.SaveChangesAsync();

        return (user, products[0].Id);
    }
}
