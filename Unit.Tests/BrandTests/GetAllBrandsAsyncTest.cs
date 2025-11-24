using System.Net;
using Application.Abstraction.Messaging;
using Application.Abstraction.Repositories;
using Application.Common.Results;
using Application.Services.Brand;
using Application.Services.Brand.DTOs.Request;
using Application.Services.Brand.DTOs.Response;
using Application.Services.Brand.Mapping;
using ArchitectureTests.FakeData;
using Domain.Entities;
using NSubstitute;
using Shouldly;

namespace Unit.Tests.BrandTests;

public class GetAllBrandsAsyncTest
{
    private readonly IBrandRepository _brandRepositoryMock;
    private readonly BrandService _brandServices;

    public GetAllBrandsAsyncTest()
    {
        _brandRepositoryMock = Substitute.For<IBrandRepository>();

        _brandServices = new BrandService(
            _brandRepositoryMock,
            Substitute.For<IUnitOfWork>(),
            Substitute.For<IEventDispatcher>());
    }

    [Fact]
    public async Task GetAllBrandsAsync_Should_ReturnBrands_When_BrandsExist()
    {
        // Arrange
        var request = new GetAllBrandsRequest();
        List<Brand> brands = new BrandFaker().Generate(3);

        IEnumerable<BrandListResponse> brandListResponses = brands.ToDto();

        _brandRepositoryMock.GetAllAsync(Arg.Any<bool>(), Arg.Any<CancellationToken>()).Returns(brands);

        // Act
        Result<IEnumerable<BrandListResponse>> result = await _brandServices
            .GetAllBrandsAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Data.ShouldNotBeNull();
        result.Error.ShouldBeNull();
        brands.Count.ShouldBe(result.Data.Count());
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        brandListResponses.ShouldBeEquivalentTo(result.Data);

        await _brandRepositoryMock.Received(1)
            .GetAllAsync(Arg.Any<bool>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetAllBrandsAsync_Should_ReturnEmptyList_When_BrandsDoesNotExist()
    {
        // Arrange
        var request = new GetAllBrandsRequest();

        _brandRepositoryMock.GetAllAsync(Arg.Any<bool>(), Arg.Any<CancellationToken>()).Returns([]);

        // Act
        Result<IEnumerable<BrandListResponse>> result = await _brandServices
            .GetAllBrandsAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Data.ShouldNotBeNull();
        result.Error.ShouldBeNull();
        result.Data.ShouldBeEmpty();
        result.Data.Count().ShouldBe(0);
        result.StatusCode.ShouldBe(HttpStatusCode.OK);

        await _brandRepositoryMock.Received(1)
            .GetAllAsync(Arg.Any<bool>(), Arg.Any<CancellationToken>());
    }
}
