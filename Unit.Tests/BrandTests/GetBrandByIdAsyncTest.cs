using System.Linq.Expressions;
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

public class GetBrandByIdAsyncTest
{
    private readonly IBrandRepository _brandRepositoryMock;
    private readonly BrandService _brandServices;

    public GetBrandByIdAsyncTest()
    {
        _brandRepositoryMock = Substitute.For<IBrandRepository>();

        _brandServices = new BrandService(
            _brandRepositoryMock,
            Substitute.For<IUnitOfWork>(),
            Substitute.For<IEventDispatcher>());
    }

    [Fact]
    public async Task GetBrandByIdAsync_Should_ReturnBrand_When_BrandExists()
    {
        // Arrange
        var brandId = Guid.CreateVersion7();
        var request = new GetBrandByIdRequest { Id = brandId };
        Brand brand = new BrandFaker().Generate();
        BrandResponse brandDto = brand.ToDto();

        _brandRepositoryMock.SingleOrDefaultAsync(
            Arg.Any<Expression<Func<Brand, bool>>>(),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(brand);

        // Act
        Result<BrandResponse> result = await _brandServices.GetBrandByIdAsync(request, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();

        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.Data.ShouldNotBeNull();
        brandDto.ShouldBeEquivalentTo(result.Data);

        await _brandRepositoryMock.Received(1)
            .SingleOrDefaultAsync(
            Arg.Any<Expression<Func<Brand, bool>>>(),
            cancellationToken: Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetBrandByIdAsync_Should_ReturnFailure_When_BrandDoesNotExists()
    {
        // Arrange
        var brandId = Guid.CreateVersion7();
        var request = new GetBrandByIdRequest { Id = brandId };

        _brandRepositoryMock.SingleOrDefaultAsync(
            Arg.Any<Expression<Func<Brand, bool>>>(),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns((Brand)null!);

        // Act
        Result<BrandResponse> result = await _brandServices.GetBrandByIdAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Data.ShouldBeNull();
        result.Error.ShouldNotBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        result.Error.Type.ShouldBe(ErrorType.NotFound);

        await _brandRepositoryMock.Received(1)
            .SingleOrDefaultAsync(Arg.Any<Expression<Func<Brand, bool>>>(),
            cancellationToken: Arg.Any<CancellationToken>());
    }
}
