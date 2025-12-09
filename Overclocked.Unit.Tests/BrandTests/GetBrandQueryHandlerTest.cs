using System.Net;
using NSubstitute;
using Overclocked.Application.Abstraction.Persistence;
using Overclocked.Application.Brand.Mapping;
using Overclocked.Application.Brand.Queries;
using Overclocked.Application.Brand.Queries.GetBrand;
using Overclocked.Application.Category.Mapping;
using Overclocked.Architecture.Tests.FakeData;
using Overclocked.Contracts.Brand;
using Overclocked.Domain.BrandAggregate;
using Overclocked.Domain.BrandAggregate.ValueObjects;
using Overclocked.Domain.Common.Enums;
using Overclocked.Domain.Common.Results;
using Shouldly;

namespace Overclocked.Unit.Tests.BrandTests;

public class GetBrandQueryHandlerTest
{
    private readonly IBrandRepository _brandRepositoryMock;
    private readonly IBrandQueries _brandQueries;

    public GetBrandQueryHandlerTest()
    {
        _brandRepositoryMock = Substitute.For<IBrandRepository>();

        _brandQueries = new BrandQueries(_brandRepositoryMock);
    }

    [Fact]
    public async Task GetBrandQueryHandler_Should_ReturnBrand_When_BrandExists()
    {
        // Arrange
        var brandId = Guid.CreateVersion7();
        var query = new GetBrandQuery { Id = BrandId.Create(brandId) };

        Brand brand = new BrandFaker().Generate();
        BrandResponse brandDto = brand.ToDto();

        _brandRepositoryMock.GetBrandByIdAsync(
            Arg.Any<BrandId>(),
            Arg.Any<CancellationToken>())
            .Returns(brand);

        // Act
        Result<BrandResponse> result = await _brandQueries.GetBrandQueryHandler(query, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBe(Error.None);

        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.Value.ShouldNotBeNull();
        brandDto.ShouldBeEquivalentTo(result.Value);

        await _brandRepositoryMock.Received(1)
            .GetBrandByIdAsync(
            Arg.Any<BrandId>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetBrandQueryHandler_Should_ReturnFailure_When_BrandDoesNotExists()
    {
        // Arrange
        var brandId = Guid.CreateVersion7();
        var query = new GetBrandQuery { Id = BrandId.Create(brandId) };

        _brandRepositoryMock.GetBrandByIdAsync(
            Arg.Any<BrandId>(),
            Arg.Any<CancellationToken>())
            .Returns((Brand)null!);

        // Act
        Result<BrandResponse> result = await _brandQueries.GetBrandQueryHandler(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);
        result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        result.Error.Type.ShouldBe(ErrorType.NotFound);

        await _brandRepositoryMock.Received(1)
            .GetBrandByIdAsync(
            Arg.Any<BrandId>(),
            Arg.Any<CancellationToken>());
    }
}
