using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Hangfire.Common;
using Hangfire.States;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Overclocked.Api.Routing;
using Overclocked.Architecture.Tests.FakeData;
using Overclocked.Domain.BrandAggregate;
using Overclocked.Domain.Common.StaticData;
using Overclocked.Infrastructure.Persistence;
using Overclocked.Integration.Tests.Shared;
using Shouldly;

namespace Overclocked.Integration.Tests.BrandTests;

[Collection(nameof(SharedTestCollection))]
public class UpdateBrandTest(CustomWebApplicationFactory factory) : IAsyncLifetime
{
    private readonly HttpClient _client = factory.HttpClient;

    public async Task InitializeAsync()
    {
        await factory.ResetDatabaseAsync();
        factory.FileStorageServiceMock.ClearReceivedCalls();
        factory.BackgroundJobClientMock.ClearReceivedCalls();

        var token = CustomWebApplicationFactory.GenerateJwtToken(
            permissions: [PermissionType.AddEditDelete.ToString()]);
        factory.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Update_Should_ReturnFailure_When_IdNotFound()
    {
        // Arrange
        const string Name = "NVIDIA";
        var id = Guid.NewGuid().ToString();
        StringContent form = CreateJsonContent(Name, "https://res.cloudinary.com/over-clocked/image.jpg");

        // Act
        HttpResponseMessage response = await _client.PutAsync(BrandRoutes.Update.Replace("{id:guid}", id), form);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Update_Should_ReturnSuccess_When_BrandExistsAndOldImageUrlIsProvided()
    {
        // Arrange
        Brand brand = await SeedDatabaseAsync();
        StringContent form = CreateJsonContent("New Name", brand.ImageUrl);

        // Act
        HttpResponseMessage response = await _client.PutAsync(
            BrandRoutes.Update.Replace("{id:guid}", brand.Id.Value.ToString()), form);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        using IServiceScope scope = factory.Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        Brand? updatedBrand = await dbContext.Brands.FindAsync(brand.Id);
        updatedBrand.ShouldNotBeNull();
        updatedBrand.Name.ShouldBe("New Name");
    }

    [Fact]
    public async Task Update_Should_ReturnSuccess_When_BrandExistsAndNewImageUrlIsProvided()
    {
        // Arrange
        Brand brand = await SeedDatabaseAsync();
        StringContent form = CreateJsonContent("New Name", "https://res.cloudinary.com/over-clocked/new-image.jpg");

        factory.BackgroundJobClientMock.Create(Arg.Any<Job>(), Arg.Any<IState>()).Returns("a-fake-job-id");

        // Act
        HttpResponseMessage response = await _client.PutAsync(
            BrandRoutes.Update.Replace("{id:guid}", brand.Id.Value.ToString()), form);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        using IServiceScope scope = factory.Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        Brand? updatedBrand = await dbContext.Brands.FindAsync(brand.Id);
        updatedBrand.ShouldNotBeNull();
        updatedBrand.Name.ShouldBe("New Name");
        updatedBrand.ImageUrl.ShouldBe("https://res.cloudinary.com/over-clocked/new-image.jpg");
    }

    [Fact]
    public async Task Update_Should_ReturnForbidden_When_UserDoesNotHavePermission()
    {
        // Arrange
        Brand brand = await SeedDatabaseAsync();
        StringContent form = CreateJsonContent("New Name", brand.ImageUrl);

        var token = CustomWebApplicationFactory.GenerateJwtToken();
        factory.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        HttpResponseMessage response = await _client.PutAsync(
            BrandRoutes.Update.Replace("{id:guid}", brand.Id.Value.ToString()), form);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    private async Task<Brand> SeedDatabaseAsync()
    {
        Brand brand = new BrandFaker().Generate();

        using IServiceScope scope = factory.Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        dbContext.Brands.Add(brand);
        await dbContext.SaveChangesAsync();

        return brand;
    }

    private static StringContent CreateJsonContent(string name, string imageUrl)
    {
        var payload = new { Name = name, ImageUrl = imageUrl };

        var json = JsonSerializer.Serialize(payload);

        return new StringContent(json, Encoding.UTF8, "application/json");
    }
}
