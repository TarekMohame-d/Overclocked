using NSubstitute;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Features.BrandUseCases.DTOs.Responses;
using Overclocked.Application.Features.BrandUseCases.GetBrandById;
using Overclocked.Application.Features.BrandUseCases.Mapping;
using Overclocked.Architecture.Tests.FakeData;
using Overclocked.Domain.BrandAggregate;
using Overclocked.Domain.BrandAggregate.ValueObjects;
using Overclocked.SharedKernel;
using Shouldly;

namespace Overclocked.Unit.Tests.BrandTests;

public class GetBrandByIdRequestHandlerTest
{
    private readonly IBrandReadRepository _brandReadRepositoryMock;
    private readonly GetBrandByIdRequestHandler _getBrandByIdRequestHandler;

    public GetBrandByIdRequestHandlerTest()
    {
        _brandReadRepositoryMock = Substitute.For<IBrandReadRepository>();

        _getBrandByIdRequestHandler = new GetBrandByIdRequestHandler(_brandReadRepositoryMock);
    }

    [Fact]
    public async Task GetBrandRequestHandler_Should_ReturnBrand_When_BrandExists()
    {
        // Arrange
        var brandId = Guid.CreateVersion7();
        var request = new GetBrandByIdRequest { Id = brandId };

        Brand brand = new BrandFaker().Generate();
        BrandResponse brandDto = brand.ToDto();

        _brandReadRepositoryMock.GetByIdAsync(Arg.Any<BrandId>(), Arg.Any<CancellationToken>()).Returns(brand);

        // Act
        Result<BrandResponse> result = await _getBrandByIdRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBe(Error.None);
        result.Value.ShouldNotBeNull();
        brandDto.ShouldBeEquivalentTo(result.Value);

        await _brandReadRepositoryMock.Received(1).GetByIdAsync(Arg.Any<BrandId>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetBrandRequestHandler_Should_ReturnFailure_When_BrandDoesNotExists()
    {
        // Arrange
        var brandId = Guid.CreateVersion7();
        var request = new GetBrandByIdRequest { Id = brandId };

        _brandReadRepositoryMock.GetByIdAsync(Arg.Any<BrandId>(), Arg.Any<CancellationToken>()).Returns((Brand)null!);

        // Act
        Result<BrandResponse> result = await _getBrandByIdRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);
        result.Error.Type.ShouldBe(ErrorType.NotFound);

        await _brandReadRepositoryMock.Received(1).GetByIdAsync(Arg.Any<BrandId>(), Arg.Any<CancellationToken>());
    }
}
