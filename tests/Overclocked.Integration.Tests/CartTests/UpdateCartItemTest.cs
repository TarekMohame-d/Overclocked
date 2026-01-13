using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Overclocked.Api.Routing;
using Overclocked.Architecture.Tests.FakeData;
using Overclocked.Domain.BrandAggregate;
using Overclocked.Domain.CartAggregate;
using Overclocked.Domain.CartAggregate.Entities;
using Overclocked.Domain.CartAggregate.ValueObjects;
using Overclocked.Domain.CategoryAggregate;
using Overclocked.Domain.ProductAggregate;
using Overclocked.Domain.TagAggregate;
using Overclocked.Domain.UserAggregate;
using Overclocked.Infrastructure.Authentication;
using Overclocked.Infrastructure.Persistence;
using Overclocked.Integration.Tests.Shared;
using Overclocked.SharedKernel;
using Shouldly;

namespace Overclocked.Integration.Tests.CartTests;

public class UpdateCartItemTest(ApiTestFixture fixture) : IAsyncLifetime
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
    public async Task UpdateCartItem_Should_ReturnForbidden_When_UserDoesNotHaveRole()
    {
        // Arrange
        StringContent form = CreateJsonContent(1);

        var token = fixture.GenerateJwtToken(role: "Admin");
        fixture.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        HttpResponseMessage response = await _client.PutAsync(
            CartRoutes.UpdateCartItem.Replace("{id:guid}", Guid.NewGuid().ToString()),
            form
        );

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdateCartItem_Should_ReturnSuccess_When_DataIsValid()
    {
        // Arrange
        (User user, CartItemId cartItemId) = await SeedDatabaseAsync();
        StringContent form = CreateJsonContent(5);

        var token = fixture.GenerateJwtToken(userId: user.Id.Value.ToString());
        fixture.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        HttpResponseMessage response = await _client.PutAsync(
            CartRoutes.UpdateCartItem.Replace("{id:guid}", cartItemId.Value.ToString()),
            form
        );

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        using IServiceScope scope = fixture.Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        Cart? cart = await dbContext.Carts.FirstOrDefaultAsync(x => x.UserId == user.Id);
        cart.ShouldNotBeNull();
        cart.CartItems.ShouldNotBeEmpty();

        CartItem? cartItem = cart.CartItems.FirstOrDefault(x => x.Id == cartItemId);
        cartItem.ShouldNotBeNull();
        cartItem.Quantity.ShouldBe(5);
    }

    private async Task<(User user, CartItemId cartItemId)> SeedDatabaseAsync()
    {
        Brand brand = new BrandFaker().Generate();
        Category category = new CategoryFaker().Generate();
        List<Tag> tags = new TagFaker().Generate(3);
        List<Product> products = new ProductFaker(brand.Id.Value, category.Id.Value, tags.ConvertAll(x => x.Id.Value)).Generate(
            5
        );
        User user = new UserFaker(new PasswordHasher()).Generate();
        var cart = Cart.Create(user.Id);
        Result<CartItemId> cartItemIdResult = cart.AddCartItem(products[0].Id, 1);
        cart.AddCartItem(products[1].Id, 4);
        cart.AddCartItem(products[2].Id, 2);

        using IServiceScope scope = fixture.Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        dbContext.Brands.Add(brand);
        dbContext.Tags.AddRange(tags);
        dbContext.Categories.Add(category);
        dbContext.Products.AddRange(products);
        dbContext.Users.Add(user);
        dbContext.Carts.Add(cart);

        await dbContext.SaveChangesAsync();

        return (user, cartItemIdResult.Value);
    }

    private static StringContent CreateJsonContent(int quantity)
    {
        var payload = new { Quantity = quantity };

        var json = JsonSerializer.Serialize(payload);

        return new StringContent(json, Encoding.UTF8, "application/json");
    }
}
