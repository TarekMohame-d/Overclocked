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
using Overclocked.Domain.CartAggregate.ValueObjects;
using Overclocked.Domain.CategoryAggregate;
using Overclocked.Domain.ProductAggregate;
using Overclocked.Domain.UserAggregate;
using Overclocked.Infrastructure.Authentication;
using Overclocked.Infrastructure.Persistence;
using Overclocked.Integration.Tests.Shared;
using Shouldly;

namespace Overclocked.Integration.Tests.CartTests;

[Collection(nameof(SharedTestCollection))]
public class AddCartItemTest(CustomWebApplicationFactory factory) : IAsyncLifetime
{
    private readonly HttpClient _client = factory.HttpClient;

    public async Task InitializeAsync()
    {
        await factory.ResetDatabaseAsync();

        var token = CustomWebApplicationFactory.GenerateJwtToken();
        factory.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task AddCartItem_Should_ReturnForbidden_When_UserDoesNotHaveRole()
    {
        // Arrange
        StringContent form = CreateJsonContent(Guid.NewGuid(), 1);

        var token = CustomWebApplicationFactory.GenerateJwtToken(role: "Admin");
        factory.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        HttpResponseMessage response = await _client.PostAsync(CartRoutes.AddCartItem, form);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task AddCartItem_Should_ReturnSuccess_When_DataIsValid()
    {
        // Arrange
        (User user, Product product) = await SeedDatabaseAsync();
        StringContent form = CreateJsonContent(product.Id.Value, 1);

        var token = CustomWebApplicationFactory.GenerateJwtToken(userId: user.Id.Value.ToString());
        factory.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        HttpResponseMessage response = await _client.PostAsync(CartRoutes.AddCartItem, form);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        using IServiceScope scope = factory.Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        Cart? cart = await dbContext.Carts.FirstOrDefaultAsync(x => x.UserId == user.Id);
        cart.ShouldNotBeNull();

        cart.CartItems.ShouldNotBeEmpty();
    }

    private async Task<(User user, Product product)> SeedDatabaseAsync()
    {
        Brand brand = new BrandFaker().Generate();
        Category category = new CategoryFaker().Generate();
        Product product = new ProductFaker(brand.Id.Value, category.Id.Value).Generate();
        User user = new UserFaker(new PasswordHasher()).Generate();
        var cart = Cart.Create(user.Id);

        using IServiceScope scope = factory.Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await dbContext.Brands.AddAsync(brand);
        await dbContext.Categories.AddAsync(category);
        await dbContext.Products.AddAsync(product);
        await dbContext.Users.AddAsync(user);
        await dbContext.Carts.AddAsync(cart);
        await dbContext.SaveChangesAsync();

        return (user, product);
    }

    private static StringContent CreateJsonContent(Guid productId, int quantity)
    {
        var payload = new { ProductId = productId, Quantity = quantity };

        var json = JsonSerializer.Serialize(payload);

        return new StringContent(json, Encoding.UTF8, "application/json");
    }
}
