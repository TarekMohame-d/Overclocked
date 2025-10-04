using System.Net;
using Application.Abstraction.Repositories;
using Application.Features.Brand.Mapping;
using Application.Features.Brand.Queries.GetAllBrands;
using ArchitectureTests.FakeData;
using NSubstitute;
using Shouldly;

namespace Unit.Tests.BrandTests.Queries;

public class GetAllBrandsQueryHandlerTest
{
    private readonly IBrandRepository _brandRepositoryMock;
    private readonly GetAllBrandsQueryHandler _handler;

    public GetAllBrandsQueryHandlerTest()
    {
        _brandRepositoryMock = Substitute.For<IBrandRepository>();
        _handler = new GetAllBrandsQueryHandler(_brandRepositoryMock);
    }

    [Fact]
    public async Task Handle_WhenBrandsExist_ShouldReturnBrands()
    {
        // Arrange
        var query = new GetAllBrandsQuery();
        var brands = new BrandFaker().Generate(3);

        var brandDtos = brands.ToDto();

        _brandRepositoryMock.GetAllAsync(Arg.Any<bool>(), Arg.Any<CancellationToken>())
            .Returns(brands);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Data.ShouldNotBeNull();
        result.Error.ShouldBeNull();
        brands.Count.ShouldBe(result.Data.Count());
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        brandDtos.ShouldBeEquivalentTo(result.Data);

        await _brandRepositoryMock.Received(1)
            .GetAllAsync(Arg.Any<bool>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenBrandsDoesNotExist_ShouldReturnEmptyList()
    {
        // Arrange
        var query = new GetAllBrandsQuery();

        _brandRepositoryMock.GetAllAsync(Arg.Any<bool>(), Arg.Any<CancellationToken>())
            .Returns([]);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

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
