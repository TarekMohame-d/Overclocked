using System.Net;
using NSubstitute;
using Overclocked.Application.Abstraction.Persistence;
using Overclocked.Application.Brand.Mapping;
using Overclocked.Application.Brand.Queries;
using Overclocked.Application.Brand.Queries.GetAllBrands;
using Overclocked.Application.Category.Mapping;
using Overclocked.Architecture.Tests.FakeData;
using Overclocked.Contracts.Brand;
using Overclocked.Domain.BrandAggregate;
using Overclocked.Domain.Common.Results;
using Shouldly;

namespace Overclocked.Unit.Tests.BrandTests;

public class GetBrandListQueryHandlerTest
{
    private readonly IBrandRepository _brandRepositoryMock;
    private readonly IBrandQueries _brandQueries;

    public GetBrandListQueryHandlerTest()
    {
        _brandRepositoryMock = Substitute.For<IBrandRepository>();

        _brandQueries = new BrandQueries(_brandRepositoryMock);
    }

    [Fact]
    public async Task GetBrandListQueryHandler_Should_ReturnBrands_When_BrandsExist()
    {
        // Arrange
        var query = new GetBrandListQuery();
        List<Brand> brands = new BrandFaker().Generate(3);

        IEnumerable<BrandListResponse> brandListResponses = brands.ToDto();

        _brandRepositoryMock.GetBrandListAsync(Arg.Any<CancellationToken>())
            .Returns(brands);

        // Act
        Result<IEnumerable<BrandListResponse>> result = await _brandQueries
            .GetBrandListQueryHandler(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Error.ShouldBe(Error.None);
        brands.Count.ShouldBe(result.Value.Count());
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        brandListResponses.ShouldBeEquivalentTo(result.Value);

        await _brandRepositoryMock.Received(1)
            .GetBrandListAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetBrandListQueryHandler_Should_ReturnEmptyList_When_BrandsDoesNotExist()
    {
        // Arrange
        var query = new GetBrandListQuery();

        _brandRepositoryMock.GetBrandListAsync(Arg.Any<CancellationToken>())
            .Returns([]);

        // Act
        Result<IEnumerable<BrandListResponse>> result = await _brandQueries
            .GetBrandListQueryHandler(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Error.ShouldBe(Error.None);
        result.Value.ShouldBeEmpty();
        result.Value.Count().ShouldBe(0);
        result.StatusCode.ShouldBe(HttpStatusCode.OK);

        await _brandRepositoryMock.Received(1)
            .GetBrandListAsync(Arg.Any<CancellationToken>());
    }
}
