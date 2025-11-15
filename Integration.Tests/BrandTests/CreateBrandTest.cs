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
using NSubstitute;
using Shouldly;

namespace Integration.Tests.BrandTests;

[Collection(nameof(SharedTestCollection))]
public class CreateBrandTest(CustomWebApplicationFactory factory) : IAsyncLifetime
{
    private readonly HttpClient _client = factory.HttpClient;

    public async Task InitializeAsync()
    {
        await factory.ResetDatabaseAsync();
        factory.FileStorageServiceMock.ClearReceivedCalls();

        var token = CustomWebApplicationFactory
            .GenerateJwtToken(permissions: [PermissionType.AddEditDelete.ToString()]);
        factory.HttpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Create_Should_CreateAndReturnSuccess_When_DataIsValid()
    {
        // Arrange
        const string Name = "NVIDIA";

        StringContent form = CreateJsonContent(Name);

        // Act
        HttpResponseMessage response = await _client.PostAsync(BrandRoutes.Create, form);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Created);

        Result? result = await response.Content.ReadFromJsonAsync<Result>();

        using IServiceScope scope = factory.Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        Brand? brand = await dbContext.Brands.SingleOrDefaultAsync(x => x.NormalizedName == Name.ToUpper());
        brand.ShouldNotBeNull();

        brand.Name.ShouldBe(Name);

        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.Created);
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBeNull();
    }

    [Fact]
    public async Task Create_Should_ReturnBadRequest_When_NameAlreadyExists()
    {
        // Arrange
        Brand brand = await SeedDatabaseAsync();
        StringContent form = CreateJsonContent(brand.Name);

        // Act
        HttpResponseMessage response = await _client.PostAsync(BrandRoutes.Create, form);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        Result? result = await response.Content.ReadFromJsonAsync<Result>();

        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBeNull();
    }

    [Fact]
    public async Task Create_Should_ReturnForbidden_When_UserDoesNotHavePermission()
    {
        // Arrange
        Brand brand = await SeedDatabaseAsync();
        StringContent form = CreateJsonContent(brand.Name);

        var token = CustomWebApplicationFactory.GenerateJwtToken();
        factory.HttpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        // Act
        HttpResponseMessage response = await _client.PostAsync(BrandRoutes.Create, form);

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

    private static StringContent CreateJsonContent(string name,
        string imageUrl = "https://res.cloudinary.com/over-clocked/image.jpg")
    {
        var payload = new
        {
            Name = name,
            ImageUrl = imageUrl
        };

        var json = JsonSerializer.Serialize(payload);

        return new StringContent(json, Encoding.UTF8, "application/json");
    }
}
