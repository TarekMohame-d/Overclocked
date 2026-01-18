using NSubstitute;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Features.BrandUseCases.DTOs.Responses;
using Overclocked.Application.Features.BrandUseCases.GetAllBrands;
using Overclocked.Application.Features.BrandUseCases.Mapping;
using Overclocked.Architecture.Tests.FakeData;
using Overclocked.Domain.BrandAggregate;
using Overclocked.SharedKernel;
using Shouldly;

namespace Overclocked.Unit.Tests.BrandTests;

public class GetAllBrandsRequestHandlerTest
{
    private readonly IBrandReadRepository _brandReadRepositoryMock;
    private readonly GetAllBrandsRequestHandler _getAllBrandsRequestHandler;

    public GetAllBrandsRequestHandlerTest()
    {
        _brandReadRepositoryMock = Substitute.For<IBrandReadRepository>();

        _getAllBrandsRequestHandler = new GetAllBrandsRequestHandler(_brandReadRepositoryMock);
    }

    [Fact]
    public async Task GetBrandListRequestHandler_Should_ReturnBrands_When_BrandsExist()
    {
        // Arrange
        var request = new GetAllBrandsRequest();
        List<Brand> brands = new BrandFaker().Generate(3);

        IEnumerable<BrandListResponse> brandListResponses = brands.ToDto();

        _brandReadRepositoryMock.GetAllAsync(Arg.Any<CancellationToken>()).Returns(brands);

        // Act
        Result<IEnumerable<BrandListResponse>> result = await _getAllBrandsRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Error.ShouldBe(Error.None);
        brands.Count.ShouldBe(result.Value.Count());

        await _brandReadRepositoryMock.Received(1).GetAllAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetBrandListRequestHandler_Should_ReturnEmptyList_When_BrandsDoesNotExist()
    {
        // Arrange
        var request = new GetAllBrandsRequest();

        _brandReadRepositoryMock.GetAllAsync(Arg.Any<CancellationToken>()).Returns([]);

        // Act
        Result<IEnumerable<BrandListResponse>> result = await _getAllBrandsRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Error.ShouldBe(Error.None);
        result.Value.ShouldBeEmpty();
        result.Value.Count().ShouldBe(0);

        await _brandReadRepositoryMock.Received(1).GetAllAsync(Arg.Any<CancellationToken>());
    }
}
