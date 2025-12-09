using System.Net;
using NSubstitute;
using Overclocked.Application.Abstraction;
using Overclocked.Application.Abstraction.Persistence;
using Overclocked.Application.Brand.Commands;
using Overclocked.Application.Brand.Commands.DeleteBrand;
using Overclocked.Architecture.Tests.FakeData;
using Overclocked.Domain.BrandAggregate;
using Overclocked.Domain.BrandAggregate.ValueObjects;
using Overclocked.Domain.Common.Enums;
using Overclocked.Domain.Common.Results;
using Shouldly;

namespace Overclocked.Unit.Tests.BrandTests;

public class DeleteBrandCommandHandlerTest
{
    private readonly IBrandRepository _brandRepositoryMock;
    private readonly IBrandCommands _brandCommands;
    private readonly IUnitOfWork _unitOfWorkMock;

    public DeleteBrandCommandHandlerTest()
    {
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _brandRepositoryMock = Substitute.For<IBrandRepository>();

        _brandCommands = new BrandCommands(_brandRepositoryMock, _unitOfWorkMock);
    }

    [Fact]
    public async Task DeleteBrandCommandHandler_Should_ReturnFailure_When_BrandDoesNotExists()
    {
        // Arrange
        var brandId = Guid.CreateVersion7();
        var command = new DeleteBrandCommand(BrandId.Create(brandId));

        _brandRepositoryMock.GetByIdAsync(Arg.Any<BrandId>(), Arg.Any<CancellationToken>())
            .Returns((Brand)null!);

        // Act
        Result result = await _brandCommands.DeleteBrandCommandHandler(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);
        result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        result.Error.Type.ShouldBe(ErrorType.NotFound);

        await _brandRepositoryMock.Received(1)
            .GetByIdAsync(Arg.Any<BrandId>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteBrandCommandHandler_Should_ReturnSuccess_When_BrandExists()
    {
        // Arrange
        var brandId = Guid.CreateVersion7();
        var command = new DeleteBrandCommand(BrandId.Create(brandId));

        Brand brand = new BrandFaker().Generate();

        _brandRepositoryMock.GetByIdAsync(Arg.Any<BrandId>(), Arg.Any<CancellationToken>())
            .Returns(brand);

        _brandRepositoryMock.Delete(Arg.Any<Brand>());

        _unitOfWorkMock.SaveChangesAsync(Arg.Any<CancellationToken>())
            .Returns(1);

        // Act
        Result result = await _brandCommands.DeleteBrandCommandHandler(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBe(Error.None);
        result.StatusCode.ShouldBe(HttpStatusCode.OK);

        await _brandRepositoryMock.Received(1)
            .GetByIdAsync(Arg.Any<BrandId>(), Arg.Any<CancellationToken>());

        await _unitOfWorkMock.Received(1)
            .SaveChangesAsync(Arg.Any<CancellationToken>());

        _brandRepositoryMock.Received(1)
            .Delete(Arg.Any<Brand>());
    }
}
