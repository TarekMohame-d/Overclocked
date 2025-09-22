using Api.Common.Routing;
using Infrastructure.Data;
using Integration.Tests.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute.ClearExtensions;
using Shouldly;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Domain.Entities;
using Application.Common.Results;
using ArchitectureTests.FakeData;

namespace Integration.Tests.BrandTests.Commands;

[Collection(nameof(SharedTestCollection))]
public class UpdateBrandTests : IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public UpdateBrandTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.HttpClient;
    }

    public async Task InitializeAsync()
    {
        await _factory.ResetDatabaseAsync();
        _factory.FileStorageServiceMock.ClearSubstitute();

        var token = _factory.GenerateJwtToken();
        _factory.HttpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
    }
    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Update_WhenIdNotValid_ShouldReturnFailure()
    {
        // Arrange
        var name = "NVIDIA";
        var id = Guid.NewGuid().ToString();
        var form = CreateMultipartFormData(name, true);

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
    public async Task Update_WhenIdValidAndOldImageUrlIsProvided_ShouldReturnSuccess()
    {
        // Arrange
        var brand = await SeedDatabaseAsync();
        var form = CreateMultipartFormData("New Name", true);

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
    public async Task Update_WhenIdValidAndImageFileIsProvided_ShouldReturnSuccess()
    {
        // Arrange
        var brand = await SeedDatabaseAsync();
        var form = CreateMultipartFormData("New Name", false, true);

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

    private async Task<Brand> SeedDatabaseAsync()
    {
        Brand brand = new BrandFaker().Generate();

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        dbContext.Brands.Add(brand);
        await dbContext.SaveChangesAsync();

        return brand;
    }

    private MultipartFormDataContent CreateMultipartFormData(
        string? name = null,
        bool includeImageUrl = false,
        bool includeImageFile = false,
        byte[]? fileBytes = null,
        string fileName = "image.jpg",
        string contentType = "image/jpeg")
    {
        fileBytes ??= "fake-image-content"u8.ToArray();

        var imageContent = new ByteArrayContent(fileBytes);
        imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse(contentType);

        var form = new MultipartFormDataContent();

        if (name is not null)
            form.Add(new StringContent(name), "Name");

        if (includeImageUrl)
            form.Add(new StringContent("https://res.cloudinary.com/over-clocked/old.jpg"), "ImageUrl");

        if (includeImageFile)
            form.Add(imageContent, "ImageFile", fileName);

        return form;
    }
}
