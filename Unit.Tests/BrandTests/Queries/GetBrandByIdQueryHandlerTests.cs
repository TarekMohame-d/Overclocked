using System.Net;
using Application.Common.Results;
using Application.Features.Brand.Mapping;
using Application.Features.Brand.Queries.GetBrandById;
using ArchitectureTests.FakeData;
using Domain.Repositories;
using NSubstitute;
using Shouldly;
using BrandEntity = Domain.Entities.Brand;

namespace Unit.Tests.BrandTests.Queries;

public class GetBrandByIdQueryHandlerTests
{
    private readonly IBrandRepository _brandRepositoryMock;
    private readonly GetBrandByIdQueryHandler _handler;

    public GetBrandByIdQueryHandlerTests()
    {
        _brandRepositoryMock = Substitute.For<IBrandRepository>();
        _handler = new GetBrandByIdQueryHandler(_brandRepositoryMock);
    }

    [Fact]
    public async Task Handle_WhenBrandExists_ShouldReturnBrand()
    {
        // Arrange
        var brandId = Guid.CreateVersion7();
        var query = new GetBrandByIdQuery { Id = brandId };
        var brand = new BrandFaker().Generate();
        var brandDto = brand.ToDto();

        _brandRepositoryMock.GetByIdAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(brand);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();

        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.Data.ShouldNotBeNull();
        brandDto.ShouldBeEquivalentTo(result.Data);

        await _brandRepositoryMock.Received(1)
            .GetByIdAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenBrandDoesNotExists_ShouldReturnFailure()
    {
        // Arrange
        var brandId = Guid.CreateVersion7();
        var query = new GetBrandByIdQuery { Id = brandId };

        _brandRepositoryMock.GetByIdAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns((BrandEntity)null!);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Data.ShouldBeNull();
        result.Error.ShouldNotBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        result.Error.Type.ShouldBe(ErrorType.NotFound);

        await _brandRepositoryMock.Received(1)
            .GetByIdAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>());
    }
}
