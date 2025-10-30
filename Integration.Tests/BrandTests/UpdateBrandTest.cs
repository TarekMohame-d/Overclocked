using Api.Common.Routing;
using Infrastructure.Data;
using Integration.Tests.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Domain.Entities;
using Application.Common.Results;
using ArchitectureTests.FakeData;
using System.Text.Json;
using System.Text;
using NSubstitute;

namespace Integration.Tests.BrandTests;

[Collection(nameof(SharedTestCollection))]
public class UpdateBrandTest : IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public UpdateBrandTest(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.HttpClient;
    }

    public async Task InitializeAsync()
    {
        await _factory.ResetDatabaseAsync();
        _factory.FileStorageServiceMock.ClearReceivedCalls();

        var token = _factory.GenerateJwtToken();
        _factory.HttpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
    }
    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Update_Should_ReturnFailure_When_IdNotFound()
    {
        // Arrange
        var name = "NVIDIA";
        var id = Guid.NewGuid().ToString();
        var form = CreateJsonContent(name);

        // Act
        var response = await _client.PutAsync(BrandRoutes.Update.Replace("{id:guid}", id), form);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);

        var result = await response.Content.ReadFromJsonAsync<Result>();

        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBeNull();
        result.Error.Type.ShouldBe(ErrorType.NotFound);
        result.Error.Description.ShouldBe("Brand not found.");
    }

    [Fact]
    public async Task Update_Should_ReturnSuccess_When_BrandExistsAndOldImageUrlIsProvided()
    {
        // Arrange
        var brand = await SeedDatabaseAsync();
        var form = CreateJsonContent("New Name", brand.Image);

        // Act
        var response = await _client.PutAsync(BrandRoutes.Update.Replace("{id:guid}", brand.Id.ToString()), form);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<Result>();

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        Brand? updatedBrand = await dbContext.Brands.FindAsync(brand.Id);
        updatedBrand.ShouldNotBeNull();
        updatedBrand.Name.ShouldBe("New Name");

        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBeNull();
    }

    [Fact]
    public async Task Update_Should_ReturnSuccess_When_BrandExistsAndNewImageUrlIsProvided()
    {
        // Arrange
        var brand = await SeedDatabaseAsync();
        var form = CreateJsonContent("New Name", "https://res.cloudinary.com/over-clocked/new-image.jpg");

        // Act
        var response = await _client.PutAsync(BrandRoutes.Update.Replace("{id:guid}", brand.Id.ToString()), form);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<Result>();

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        Brand? updatedBrand = await dbContext.Brands.FindAsync(brand.Id);
        updatedBrand.ShouldNotBeNull();
        updatedBrand.Name.ShouldBe("New Name");
        updatedBrand.Image.ShouldBe("https://res.cloudinary.com/over-clocked/new-image.jpg");

        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBeNull();

        await _factory.FileStorageServiceMock.Received(1)
            .DeleteFileAsync(
            Arg.Is<string>(url => url == "https://res.cloudinary.com/over-clocked/image.jpg"),
            Arg.Any<CancellationToken>());
    }

    private async Task<Brand> SeedDatabaseAsync()
    {
        Brand brand = new BrandFaker().Generate();
        brand.Image = "https://res.cloudinary.com/over-clocked/image.jpg";

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        dbContext.Brands.Add(brand);
        await dbContext.SaveChangesAsync();

        return brand;
    }

    private StringContent CreateJsonContent(string name, string imageUrl = "https://res.cloudinary.com/over-clocked/image.jpg")
    {
        var payload = new
        {
            Name = name,
            ImageUrl = imageUrl
        };

        string json = JsonSerializer.Serialize(payload);

        return new StringContent(json, Encoding.UTF8, "application/json");
    }
}
