using System.Linq.Expressions;
using NSubstitute;
using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Brand.Commands.UpdateBrand;
using Overclocked.Architecture.Tests.FakeData;
using Overclocked.Domain.BrandAggregate;
using Overclocked.Domain.BrandAggregate.ValueObjects;
using Overclocked.Domain.Common.Enums;
using Overclocked.Domain.Common.Results;
using Shouldly;

namespace Overclocked.Unit.Tests.BrandTests;

public class UpdateBrandCommandHandlerTest
{
    private readonly IBrandRepository _brandRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly UpdateBrandCommandHandler _updateBrandCommandHandler;

    public UpdateBrandCommandHandlerTest()
    {
        _brandRepositoryMock = Substitute.For<IBrandRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();

        _updateBrandCommandHandler = new UpdateBrandCommandHandler(_brandRepositoryMock, _unitOfWorkMock);
    }

    [Fact]
    public async Task UpdateBrandCommandHandler_Should_ReturnFailure_When_BrandDoesNotExists()
    {
        // Arrange
        var brandId = Guid.NewGuid();
        var command = new UpdateBrandCommand
        {
            Id = brandId,
            Name = "Brand Name",
            ImageUrl = "image.png"
        };

        _brandRepositoryMock.FindAsync(Arg.Any<BrandId>(), Arg.Any<CancellationToken>())
            .Returns((Brand)null!);

        // Act
        Result result = await _updateBrandCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);
        result.Error.Type.ShouldBe(ErrorType.NotFound);

        await _brandRepositoryMock.Received(1)
            .FindAsync(Arg.Any<BrandId>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateBrandCommandHandler_Should_ReturnSuccess_When_BrandExistAndNameIsSame()
    {
        // Arrange
        var brandId = Guid.NewGuid();
        var command = new UpdateBrandCommand
        {
            Id = brandId,
            Name = "Brand Name",
            ImageUrl = "image.png"
        };

        Brand brand = new BrandFaker().Generate();

        _brandRepositoryMock.FindAsync(Arg.Any<BrandId>(), Arg.Any<CancellationToken>())
            .Returns(brand);

        _brandRepositoryMock.AnyAsync(Arg.Any<Expression<Func<Brand, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(false);

        _unitOfWorkMock.SaveChangesAsync(Arg.Any<CancellationToken>())
            .Returns(1);

        // Act
        Result result = await _updateBrandCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBe(Error.None);

        await _brandRepositoryMock.Received(1)
            .FindAsync(Arg.Any<BrandId>(), Arg.Any<CancellationToken>());

        await _brandRepositoryMock.Received(1)
            .AnyAsync(Arg.Any<Expression<Func<Brand, bool>>>(), Arg.Any<CancellationToken>());

        await _unitOfWorkMock.Received(1)
            .SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateBrandCommandHandler_Should_ReturnFailure_When_BrandExistAndNewNameAlreadyExists()
    {
        // Arrange
        var brandId = Guid.NewGuid();
        var command = new UpdateBrandCommand
        {
            Id = brandId,
            Name = "Brand Name",
            ImageUrl = "image.png"
        };

        Brand brand = new BrandFaker().Generate();

        _brandRepositoryMock.FindAsync(Arg.Any<BrandId>(), Arg.Any<CancellationToken>())
            .Returns(brand);

        _brandRepositoryMock.AnyAsync(Arg.Any<Expression<Func<Brand, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(true);

        // Act
        Result result = await _updateBrandCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);

        await _brandRepositoryMock.Received(1)
            .FindAsync(Arg.Any<BrandId>(), Arg.Any<CancellationToken>());

        await _brandRepositoryMock.Received(1)
            .AnyAsync(Arg.Any<Expression<Func<Brand, bool>>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateBrandCommandHandler_Should_ReturnSuccess_When_BrandExistAndNameChangedAndNameIsUnique()
    {
        // Arrange
        var brandId = Guid.NewGuid();
        var command = new UpdateBrandCommand
        {
            Id = brandId,
            Name = "Brand Name",
            ImageUrl = "image.png"
        };

        Brand brand = new BrandFaker().Generate();

        _brandRepositoryMock.FindAsync(Arg.Any<BrandId>(), Arg.Any<CancellationToken>())
            .Returns(brand);

        _brandRepositoryMock.AnyAsync(Arg.Any<Expression<Func<Brand, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        Result result = await _updateBrandCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBe(Error.None);

        await _brandRepositoryMock.Received(1)
            .FindAsync(Arg.Any<BrandId>(), Arg.Any<CancellationToken>());

        await _brandRepositoryMock.Received(1)
            .AnyAsync(Arg.Any<Expression<Func<Brand, bool>>>(), Arg.Any<CancellationToken>());

        await _unitOfWorkMock.Received(1)
            .SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
