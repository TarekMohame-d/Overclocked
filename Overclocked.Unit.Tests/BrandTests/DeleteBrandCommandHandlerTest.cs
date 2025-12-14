using NSubstitute;
using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Persistence;
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
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly DeleteBrandCommandHandler _deleteBrandCommandHandler;

    public DeleteBrandCommandHandlerTest()
    {
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _brandRepositoryMock = Substitute.For<IBrandRepository>();

        _deleteBrandCommandHandler = new DeleteBrandCommandHandler(_brandRepositoryMock, _unitOfWorkMock);
    }

    [Fact]
    public async Task DeleteBrandCommandHandler_Should_ReturnFailure_When_BrandDoesNotExists()
    {
        // Arrange
        var brandId = Guid.CreateVersion7();
        var command = new DeleteBrandCommand
        {
            Id = brandId
        };

        _brandRepositoryMock.FindAsync(Arg.Any<BrandId>(), Arg.Any<CancellationToken>())
            .Returns((Brand)null!);

        // Act
        Result result = await _deleteBrandCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);
        result.Error.Type.ShouldBe(ErrorType.NotFound);

        await _brandRepositoryMock.Received(1)
            .FindAsync(Arg.Any<BrandId>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteBrandCommandHandler_Should_ReturnSuccess_When_BrandExists()
    {
        // Arrange
        var brandId = Guid.CreateVersion7();
        var command = new DeleteBrandCommand
        {
            Id = brandId
        };

        Brand brand = new BrandFaker().Generate();

        _brandRepositoryMock.FindAsync(Arg.Any<BrandId>(), Arg.Any<CancellationToken>())
            .Returns(brand);

        _brandRepositoryMock.Delete(Arg.Any<Brand>());

        _unitOfWorkMock.SaveChangesAsync(Arg.Any<CancellationToken>())
            .Returns(1);

        // Act
        Result result = await _deleteBrandCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBe(Error.None);

        await _brandRepositoryMock.Received(1)
            .FindAsync(Arg.Any<BrandId>(), Arg.Any<CancellationToken>());

        await _unitOfWorkMock.Received(1)
            .SaveChangesAsync(Arg.Any<CancellationToken>());

        _brandRepositoryMock.Received(1)
            .Delete(Arg.Any<Brand>());
    }
}
