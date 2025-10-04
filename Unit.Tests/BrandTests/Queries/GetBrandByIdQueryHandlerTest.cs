using System.Net;
using Application.Abstraction.Repositories;
using Application.Common.Results;
using Application.Features.Brand.Mapping;
using Application.Features.Brand.Queries.GetBrandById;
using ArchitectureTests.FakeData;
using Domain.Entities;
using NSubstitute;
using Shouldly;

namespace Unit.Tests.BrandTests.Queries;

public class GetBrandByIdQueryHandlerTest
{
    private readonly IBrandRepository _brandRepositoryMock;
    private readonly GetBrandByIdQueryHandler _handler;

    public GetBrandByIdQueryHandlerTest()
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
            .Returns((Brand)null!);

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
