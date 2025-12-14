using System.Net;
using NSubstitute;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Brand.Mapping;
using Overclocked.Application.Brand.Queries.GetAllBrands;
using Overclocked.Application.Category.Mapping;
using Overclocked.Architecture.Tests.FakeData;
using Overclocked.Contracts.Brand;
using Overclocked.Domain.BrandAggregate;
using Overclocked.Domain.Common.Results;
using Shouldly;

namespace Overclocked.Unit.Tests.BrandTests;

public class GetAllBrandsQueryHandlerTest
{
    private readonly IBrandRepository _brandRepositoryMock;
    private readonly GetAllBrandsQueryHandler _getAllBrandsQueryHandler;

    public GetAllBrandsQueryHandlerTest()
    {
        _brandRepositoryMock = Substitute.For<IBrandRepository>();

        _getAllBrandsQueryHandler = new GetAllBrandsQueryHandler(_brandRepositoryMock);
    }

    [Fact]
    public async Task GetBrandListQueryHandler_Should_ReturnBrands_When_BrandsExist()
    {
        // Arrange
        var query = new GetAllBrandsQuery();
        List<Brand> brands = new BrandFaker().Generate(3);

        IEnumerable<BrandListResponse> brandListResponses = brands.ToDto();

        _brandRepositoryMock.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(brands);

        // Act
        Result<IEnumerable<BrandListResponse>> result = await _getAllBrandsQueryHandler
            .Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Error.ShouldBe(Error.None);
        brands.Count.ShouldBe(result.Value.Count());
        brandListResponses.ShouldBeEquivalentTo(result.Value);

        await _brandRepositoryMock.Received(1)
            .GetAllAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetBrandListQueryHandler_Should_ReturnEmptyList_When_BrandsDoesNotExist()
    {
        // Arrange
        var query = new GetAllBrandsQuery();

        _brandRepositoryMock.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns([]);

        // Act
        Result<IEnumerable<BrandListResponse>> result = await _getAllBrandsQueryHandler
            .Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Error.ShouldBe(Error.None);
        result.Value.ShouldBeEmpty();
        result.Value.Count().ShouldBe(0);

        await _brandRepositoryMock.Received(1)
            .GetAllAsync(Arg.Any<CancellationToken>());
    }
}
