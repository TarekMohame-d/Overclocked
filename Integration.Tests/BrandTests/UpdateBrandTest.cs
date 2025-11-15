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
using Hangfire.Common;
using Hangfire.States;
using Infrastructure.Data;
using Integration.Tests.Shared;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Shouldly;

namespace Integration.Tests.BrandTests;

[Collection(nameof(SharedTestCollection))]
public class UpdateBrandTest(CustomWebApplicationFactory factory) : IAsyncLifetime
{
    private readonly HttpClient _client = factory.HttpClient;

    public async Task InitializeAsync()
    {
        await factory.ResetDatabaseAsync();
        factory.FileStorageServiceMock.ClearReceivedCalls();
        factory.BackgroundJobClientMock.ClearReceivedCalls();

        var token = CustomWebApplicationFactory
            .GenerateJwtToken(permissions: [PermissionType.AddEditDelete.ToString()]);
        factory.HttpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Update_Should_ReturnFailure_When_IdNotFound()
    {
        // Arrange
        const string Name = "NVIDIA";
        var id = Guid.NewGuid().ToString();
        StringContent form = CreateJsonContent(Name);

        // Act
        HttpResponseMessage response = await _client.PutAsync(BrandRoutes.Update.Replace("{id:guid}", id), form);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);

        Result? result = await response.Content.ReadFromJsonAsync<Result>();

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
        Brand brand = await SeedDatabaseAsync();
        StringContent form = CreateJsonContent("New Name", brand.Image);

        // Act
        HttpResponseMessage response =
            await _client.PutAsync(BrandRoutes.Update.Replace("{id:guid}", brand.Id.ToString()), form);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        Result? result = await response.Content.ReadFromJsonAsync<Result>();

        using IServiceScope scope = factory.Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

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
        Brand brand = await SeedDatabaseAsync();
        StringContent form = CreateJsonContent("New Name", "https://res.cloudinary.com/over-clocked/new-image.jpg");

        factory.BackgroundJobClientMock
            .Create(Arg.Any<Job>(), Arg.Any<IState>())
            .Returns("a-fake-job-id");

        // Act
        HttpResponseMessage response =
            await _client.PutAsync(BrandRoutes.Update.Replace("{id:guid}", brand.Id.ToString()), form);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        Result? result = await response.Content.ReadFromJsonAsync<Result>();

        using IServiceScope scope = factory.Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        Brand? updatedBrand = await dbContext.Brands.FindAsync(brand.Id);
        updatedBrand.ShouldNotBeNull();
        updatedBrand.Name.ShouldBe("New Name");
        updatedBrand.Image.ShouldBe("https://res.cloudinary.com/over-clocked/new-image.jpg");

        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBeNull();

        factory.BackgroundJobClientMock.Received(1)
            .Create(Arg.Any<Job>(), Arg.Any<EnqueuedState>());
    }

    [Fact]
    public async Task Update_Should_ReturnForbidden_When_UserDoesNotHavePermission()
    {
        // Arrange
        Brand brand = await SeedDatabaseAsync();
        StringContent form = CreateJsonContent("New Name", brand.Image);

        var token = CustomWebApplicationFactory.GenerateJwtToken();
        factory.HttpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        // Act
        HttpResponseMessage response =
            await _client.PutAsync(BrandRoutes.Update.Replace("{id:guid}", brand.Id.ToString()), form);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    private async Task<Brand> SeedDatabaseAsync()
    {
        Brand brand = new BrandFaker().Generate();
        brand.Image = "https://res.cloudinary.com/over-clocked/image.jpg";

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
