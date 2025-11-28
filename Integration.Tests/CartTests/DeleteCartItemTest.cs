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

namespace Integration.Tests.CartTests;

[Collection(nameof(SharedTestCollection))]
public class DeleteCartItemTest(CustomWebApplicationFactory factory) : IAsyncLifetime
{
    private readonly HttpClient _client = factory.HttpClient;

    public async Task InitializeAsync() => await factory.ResetDatabaseAsync();

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task DeleteCartItem_Should_ReturnSuccess_When_DataIsValid()
    {
        // Arrange
        (User user, Product product, Product product2, Cart cart) = await SeedDatabaseAsync(10);

        var token = CustomWebApplicationFactory.GenerateJwtToken(user.Id.ToString());
        factory.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        HttpResponseMessage response = await _client
            .DeleteAsync(CartRoutes.DeleteCartItem.Replace("{id:guid}", cart.CartItems.First().Id.ToString()));

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        Result? result = await response.Content.ReadFromJsonAsync<Result>();

        using IServiceScope scope = factory.Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        Cart? cartDb = await dbContext.Carts.Include(x => x.CartItems)
            .SingleOrDefaultAsync(x => x.UserId == user.Id);

        cartDb.ShouldNotBeNull();
        cartDb.CartItems.Count.ShouldBe(1);
        cartDb.CartItems.Any(ci => ci.ProductId == product2.Id).ShouldBeTrue();

        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBeNull();
    }

    private async Task<(User user, Product product, Product product2, Cart cart)> SeedDatabaseAsync(int quantity = 10)
    {
        User user = new UserFaker().Generate();
        Brand brand = new BrandFaker().Generate();
        Category category = new CategoryFaker().Generate();
        Product product = new ProductFaker().Generate();
        Product product2 = new ProductFaker().Generate();
        var cart = new Cart { User = user, UserId = user.Id };

        cart.CartItems.Add(new CartItem
        {
            CartId = cart.Id,
            ProductId = product.Id,
            Quantity = 2
        });

        cart.CartItems.Add(new CartItem
        {
            CartId = cart.Id,
            ProductId = product2.Id,
            Quantity = 1
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
        dbContext.Carts.Add(cart);
        await dbContext.SaveChangesAsync();

        return (user, product, product2, cart);
    }
}
