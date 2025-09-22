using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Api.Common.Routing;
using Application.Common.Results;
using ArchitectureTests.FakeData;
using Infrastructure.Data;
using Integration.Tests.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute.ClearExtensions;
using Shouldly;
using Domain.Entities;

namespace Integration.Tests.BrandTests.Commands;

[Collection(nameof(SharedTestCollection))]
public class CreateBrandTests : IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public CreateBrandTests(CustomWebApplicationFactory factory)
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
    public async Task Create_WhenDataIsValid_ShouldCreateAndReturnSuccess()
    {
        // Arrange
        var name = "NVIDIA";

        var form = CreateMultipartFormData(name);

        // Act
        var response = await _client.PostAsync(BrandRoutes.Create, form);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Created);

        var result = await response.Content.ReadFromJsonAsync<Result>();

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var brand = await dbContext.Brands.SingleOrDefaultAsync(x => x.Name == name);
        brand.ShouldNotBeNull();

        brand.Name.ShouldBe(name);

        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.Created);
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBeNull();
    }

    [Fact]
    public async Task Create_WhenNameAlreadyExists_ShouldReturnBadRequest()
    {
        // Arrange
        var brand = await SeedDatabaseAsync();
        var form = CreateMultipartFormData(brand.Name);

        // Act
        var response = await _client.PostAsync(BrandRoutes.Create, form);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        var result = await response.Content.ReadFromJsonAsync<Result>();

        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBeNull();
    }

    private async Task<Brand> SeedDatabaseAsync()
    {
        var brand = new BrandFaker().Generate();

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        dbContext.Brands.Add(brand);
        await dbContext.SaveChangesAsync();

        return brand;
    }

    private MultipartFormDataContent CreateMultipartFormData(
        string name,
        byte[]? fileBytes = null,
        string fileName = "image.jpg",
        string contentType = "image/jpeg")
    {
        fileBytes ??= "fake-image-content"u8.ToArray();

        var imageContent = new ByteArrayContent(fileBytes);
        imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse(contentType);

        var form = new MultipartFormDataContent
            {
                { new StringContent(name), "Name" },
                { imageContent, "ImageFile", fileName }
            };

        return form;
    }
}
