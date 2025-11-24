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

namespace Integration.Tests.CartTests;

[Collection(nameof(SharedTestCollection))]
public class AddCartItemTest(CustomWebApplicationFactory factory) : IAsyncLifetime
{
    private readonly HttpClient _client = factory.HttpClient;

    public async Task InitializeAsync() => await factory.ResetDatabaseAsync();

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task AddCartItem_Should_ReturnSuccess_When_DataIsValid()
    {
        // Arrange
        (User user, Product product, Cart cart) = await SeedDatabaseAsync();

        var token = CustomWebApplicationFactory.GenerateJwtToken(user.Id.ToString());
        factory.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        StringContent form = CreateJsonContent(product.Id, 2);

        // Act
        HttpResponseMessage response = await _client.PostAsync(CartRoutes.AddCartItem, form);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        Result? result = await response.Content.ReadFromJsonAsync<Result>();

        using IServiceScope scope = factory.Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        CartItem? cartItem = await dbContext.CartItems
            .SingleOrDefaultAsync(x => x.CartId == cart.Id && x.ProductId == product.Id);

        cartItem.ShouldNotBeNull();
        cartItem.Quantity.ShouldBe(2);
        cartItem.ProductId.ShouldBe(product.Id);

        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBeNull();
    }

    [Fact]
    public async Task AddCartItem_Should_ReturnFailure_When_QuantityIsInvalid()
    {
        // Arrange
        (User user, Product product, Cart cart) = await SeedDatabaseAsync(10);

        var token = CustomWebApplicationFactory.GenerateJwtToken(user.Id.ToString());
        factory.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        StringContent form = CreateJsonContent(product.Id, 20);

        // Act
        HttpResponseMessage response = await _client.PostAsync(CartRoutes.AddCartItem, form);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        Result? result = await response.Content.ReadFromJsonAsync<Result>();

        using IServiceScope scope = factory.Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        CartItem? cartItem = await dbContext.CartItems
            .SingleOrDefaultAsync(x => x.CartId == cart.Id && x.ProductId == product.Id);

        cartItem.ShouldBeNull();

        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBeNull();
    }

    private async Task<(User user, Product product, Cart cart)> SeedDatabaseAsync(int quantity = 10)
    {
        User user = new UserFaker().Generate();
        Brand brand = new BrandFaker().Generate();
        Category category = new CategoryFaker().Generate();
        Product product = new ProductFaker().Generate();
        var cart = new Cart { User = user, UserId = user.Id };

        var role = new Role { Name = "Customer", Id = 4 };

        user.RoleType = RoleType.Customer;

        product.CategoryId = category.Id;
        product.BrandId = brand.Id;
        product.StockQuantity = quantity;

        using IServiceScope scope = factory.Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        dbContext.Brands.Add(brand);
        dbContext.Categories.Add(category);
        dbContext.Products.Add(product);
        dbContext.Roles.Add(role);
        dbContext.Users.Add(user);
        dbContext.Carts.Add(cart);
        await dbContext.SaveChangesAsync();

        return (user, product, cart);
    }

    private static StringContent CreateJsonContent(Guid productId, int quantity)
    {
        var payload = new { ProductId = productId, Quantity = quantity };

        var json = JsonSerializer.Serialize(payload);

        return new StringContent(json, Encoding.UTF8, "application/json");
    }
}
