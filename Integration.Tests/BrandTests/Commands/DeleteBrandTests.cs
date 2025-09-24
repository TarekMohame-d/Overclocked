using Api.Common.Routing;
using ArchitectureTests.FakeData;
using Infrastructure.Data;
using Integration.Tests.Shared;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute.ClearExtensions;
using Shouldly;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Domain.Entities;
using Application.Common.Results;

namespace Integration.Tests.BrandTests.Commands;

[Collection(nameof(SharedTestCollection))]
public class DeleteBrandTests : IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public DeleteBrandTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.HttpClient;
    }

    public async Task InitializeAsync()
    {
        await _factory.ResetDatabaseAsync();

        var token = _factory.GenerateJwtToken();
        _factory.HttpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
    }
    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Delete_WhenIdNotValid_ShouldReturnFailure()
    {
        // Arrange
        var brandId = Guid.NewGuid().ToString();

        // Act
        var response = await _client.DeleteAsync(BrandRoutes.Delete.Replace("{id:guid}", brandId));

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
    public async Task Delete_WhenIdIsValid_ShouldReturnSuccess()
    {
        // Arrange
        var brand = await SeedDatabaseAsync();

        // Act
        var response = await _client.DeleteAsync(BrandRoutes.Delete.Replace("{id:guid}", brand.Id.ToString()));

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<Result>();

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        Brand? deletedBrand = await dbContext.Brands.FindAsync(brand.Id);

        deletedBrand.ShouldBeNull();

        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBeNull();
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
}
