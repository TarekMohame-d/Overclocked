using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Overclocked.Api.Routing;
using Overclocked.Application.Features.WishlistUseCases.DTOs.Responses;
using Overclocked.Architecture.Tests.FakeData;
using Overclocked.Domain.BrandAggregate;
using Overclocked.Domain.CategoryAggregate;
using Overclocked.Domain.ProductAggregate;
using Overclocked.Domain.TagAggregate;
using Overclocked.Domain.UserAggregate;
using Overclocked.Domain.WishlistAggregate;
using Overclocked.Infrastructure.Authentication;
using Overclocked.Infrastructure.Persistence;
using Overclocked.Integration.Tests.Shared;
using Shouldly;

namespace Overclocked.Integration.Tests.WishlistTests;

[Collection(nameof(IntegrationTestCollection))]
public class GetWishlistItemsTest(IntegrationTestWebAppFactory factory) : IAsyncLifetime
{
    private readonly HttpClient _client = factory.HttpClient;

    public async Task InitializeAsync()
    {
        await factory.ResetDatabaseAsync();

        var token = factory.GenerateJwtToken();
        factory.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task GetWishlistItems_Should_ReturnForbidden_When_UserDoesNotHaveRole()
    {
        // Arrange
        var token = factory.GenerateJwtToken(role: "Admin");
        factory.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        HttpResponseMessage response = await _client.GetAsync(WishlistRoutes.GetWishlistItems);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetWishlistItems_Should_ReturnSuccess_When_DataIsValid()
    {
        // Arrange
        User user = await SeedDatabaseAsync();

        var token = factory.GenerateJwtToken(userId: user.Id.Value.ToString());
        factory.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        HttpResponseMessage response = await _client.GetAsync(WishlistRoutes.GetWishlistItems);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        IEnumerable<WishlistItemResponse>? result = await response.Content.ReadFromJsonAsync<IEnumerable<WishlistItemResponse>>();

        result.ShouldNotBeNull();
        result.ShouldNotBeEmpty();
        result.Count().ShouldBe(3);
    }

    private async Task<User> SeedDatabaseAsync()
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

        using IServiceScope scope = factory.Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        dbContext.Brands.Add(brand);
        dbContext.Tags.Add(tag);
        dbContext.Categories.Add(category);
        dbContext.Products.AddRange(products);
        dbContext.Users.Add(user);
        dbContext.Wishlists.Add(wishlist);

        await dbContext.SaveChangesAsync();

        return user;
    }
}
