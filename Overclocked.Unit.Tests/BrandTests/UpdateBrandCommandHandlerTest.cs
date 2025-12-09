using System.Linq.Expressions;
using System.Net;
using NSubstitute;
using Overclocked.Application.Abstraction;
using Overclocked.Application.Abstraction.Persistence;
using Overclocked.Application.Brand.Commands;
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
    private readonly IBrandCommands _brandCommands;
    private readonly IUnitOfWork _unitOfWorkMock;

    public UpdateBrandCommandHandlerTest()
    {
        _brandRepositoryMock = Substitute.For<IBrandRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();

        _brandCommands = new BrandCommands(_brandRepositoryMock, _unitOfWorkMock);
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

        _brandRepositoryMock.SingleOrDefaultAsync(
            Arg.Any<Expression<Func<Brand, bool>>>(),
            asNoTracking: Arg.Any<bool>(),
            Arg.Any<CancellationToken>())
            .Returns((Brand)null!);

        // Act
        Result result = await _brandCommands.UpdateBrandCommandHandler(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        result.Error.ShouldNotBe(Error.None);
        result.Error.Type.ShouldBe(ErrorType.NotFound);

        await _brandRepositoryMock.Received(1)
            .SingleOrDefaultAsync(
            Arg.Any<Expression<Func<Brand, bool>>>(),
            asNoTracking: Arg.Any<bool>(),
            Arg.Any<CancellationToken>());
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

        _brandRepositoryMock.SingleOrDefaultAsync(
            Arg.Any<Expression<Func<Brand, bool>>>(),
            asNoTracking: Arg.Any<bool>(),
            Arg.Any<CancellationToken>())
            .Returns(brand);

        _brandRepositoryMock.AnyAsync(Arg.Any<Expression<Func<Brand, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(false);

        _unitOfWorkMock.SaveChangesAsync(Arg.Any<CancellationToken>())
            .Returns(1);

        // Act
        Result result = await _brandCommands.UpdateBrandCommandHandler(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.Error.ShouldBe(Error.None);

        await _brandRepositoryMock.Received(1)
            .SingleOrDefaultAsync(
            Arg.Any<Expression<Func<Brand, bool>>>(),
            asNoTracking: Arg.Any<bool>(),
            Arg.Any<CancellationToken>());

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

        _brandRepositoryMock.SingleOrDefaultAsync(
            Arg.Any<Expression<Func<Brand, bool>>>(),
            asNoTracking: Arg.Any<bool>(),
            Arg.Any<CancellationToken>())
            .Returns(brand);

        _brandRepositoryMock.AnyAsync(Arg.Any<Expression<Func<Brand, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(true);

        // Act
        Result result = await _brandCommands.UpdateBrandCommandHandler(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.StatusCode.ShouldBe(HttpStatusCode.Conflict);
        result.Error.ShouldNotBe(Error.None);

        await _brandRepositoryMock.Received(1)
            .SingleOrDefaultAsync(
            Arg.Any<Expression<Func<Brand, bool>>>(),
            asNoTracking: Arg.Any<bool>(),
            Arg.Any<CancellationToken>());

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

        _brandRepositoryMock.SingleOrDefaultAsync(
            Arg.Any<Expression<Func<Brand, bool>>>(),
            asNoTracking: Arg.Any<bool>(),
            Arg.Any<CancellationToken>())
            .Returns(brand);

        _brandRepositoryMock.AnyAsync(Arg.Any<Expression<Func<Brand, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        Result result = await _brandCommands.UpdateBrandCommandHandler(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.Error.ShouldBe(Error.None);

        await _brandRepositoryMock.Received(1)
            .SingleOrDefaultAsync(
            Arg.Any<Expression<Func<Brand, bool>>>(),
            asNoTracking: Arg.Any<bool>(),
            Arg.Any<CancellationToken>());

        await _brandRepositoryMock.Received(1)
            .AnyAsync(Arg.Any<Expression<Func<Brand, bool>>>(), Arg.Any<CancellationToken>());

        await _unitOfWorkMock.Received(1)
            .SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
