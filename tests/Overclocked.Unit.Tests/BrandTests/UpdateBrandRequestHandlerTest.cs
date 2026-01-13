using NSubstitute;
using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Features.BrandUseCases.UpdateBrand;
using Overclocked.Architecture.Tests.FakeData;
using Overclocked.Domain.BrandAggregate;
using Overclocked.Domain.BrandAggregate.ValueObjects;
using Overclocked.SharedKernel;
using Shouldly;

namespace Overclocked.Unit.Tests.BrandTests;

public class UpdateBrandRequestHandlerTest
{
    private readonly IBrandRepository _brandRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly UpdateBrandRequestHandler _updateBrandRequestHandler;

    public UpdateBrandRequestHandlerTest()
    {
        _brandRepositoryMock = Substitute.For<IBrandRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();

        _updateBrandRequestHandler = new UpdateBrandRequestHandler(_brandRepositoryMock, _unitOfWorkMock);
    }

    [Fact]
    public async Task UpdateBrandRequestHandler_Should_ReturnFailure_When_ImageUrlIsInvalid()
    {
        // Arrange
        var brandId = Guid.NewGuid();
        var request = new UpdateBrandRequest
        {
            Id = brandId,
            Name = "Brand Name",
            ImageUrl = "https://res.cloudinary.com/over-clocked/brands/image.jpg",
        };

        // Act
        Result result = await _updateBrandRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);
    }

    [Fact]
    public async Task UpdateBrandRequestHandler_Should_ReturnFailure_When_NameIsInvalid()
    {
        // Arrange
        var brandId = Guid.NewGuid();
        var request = new UpdateBrandRequest
        {
            Id = brandId,
            Name = "  ",
            ImageUrl = "https://res.cloudinary.com/over-clocked/brands/image.jpg",
        };

        Brand brand = new BrandFaker().Generate();

        _brandRepositoryMock.GetByIdAsync(Arg.Any<BrandId>(), Arg.Any<CancellationToken>()).Returns(brand);

        _brandRepositoryMock.NameExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(false);

        // Act
        Result result = await _updateBrandRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);

        await _brandRepositoryMock.Received(1).GetByIdAsync(Arg.Any<BrandId>(), Arg.Any<CancellationToken>());

        await _brandRepositoryMock.Received(1).NameExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateBrandRequestHandler_Should_ReturnFailure_When_BrandDoesNotExists()
    {
        // Arrange
        var brandId = Guid.NewGuid();
        var request = new UpdateBrandRequest
        {
            Id = brandId,
            Name = "Brand Name",
            ImageUrl = "https://res.cloudinary.com/over-clocked/brands/image.jpg",
        };

        _brandRepositoryMock.GetByIdAsync(Arg.Any<BrandId>(), Arg.Any<CancellationToken>()).Returns((Brand)null!);

        // Act
        Result result = await _updateBrandRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);
        result.Error.Type.ShouldBe(ErrorType.NotFound);

        await _brandRepositoryMock.Received(1).GetByIdAsync(Arg.Any<BrandId>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateBrandRequestHandler_Should_ReturnSuccess_When_BrandExistWithSameName()
    {
        // Arrange
        Brand brand = new BrandFaker().Generate();

        var request = new UpdateBrandRequest
        {
            Id = brand.Id.Value,
            Name = brand.Name,
            ImageUrl = "https://res.cloudinary.com/over-clocked/brands/image.jpg",
        };

        _brandRepositoryMock.GetByIdAsync(Arg.Any<BrandId>(), Arg.Any<CancellationToken>()).Returns(brand);

        _unitOfWorkMock.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        Result result = await _updateBrandRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBe(Error.None);

        await _brandRepositoryMock.Received(1).GetByIdAsync(Arg.Any<BrandId>(), Arg.Any<CancellationToken>());

        await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateBrandRequestHandler_Should_ReturnFailure_When_BrandExistAndNewNameAlreadyExists()
    {
        // Arrange
        var brandId = Guid.NewGuid();
        var request = new UpdateBrandRequest
        {
            Id = brandId,
            Name = "Brand Name",
            ImageUrl = "https://res.cloudinary.com/over-clocked/brands/image.jpg",
        };

        Brand brand = new BrandFaker().Generate();

        _brandRepositoryMock.GetByIdAsync(Arg.Any<BrandId>(), Arg.Any<CancellationToken>()).Returns(brand);

        _brandRepositoryMock.NameExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(true);

        // Act
        Result result = await _updateBrandRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);

        await _brandRepositoryMock.Received(1).GetByIdAsync(Arg.Any<BrandId>(), Arg.Any<CancellationToken>());

        await _brandRepositoryMock.Received(1).NameExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateBrandRequestHandler_Should_ReturnSuccess_When_BrandExistAndNewNameIsUnique()
    {
        // Arrange
        var brandId = Guid.NewGuid();
        var request = new UpdateBrandRequest
        {
            Id = brandId,
            Name = "Brand Name",
            ImageUrl = "https://res.cloudinary.com/over-clocked/brands/image.jpg",
        };

        Brand brand = new BrandFaker().Generate();

        _brandRepositoryMock.GetByIdAsync(Arg.Any<BrandId>(), Arg.Any<CancellationToken>()).Returns(brand);

        _brandRepositoryMock.NameExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(false);

        // Act
        Result result = await _updateBrandRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBe(Error.None);

        await _brandRepositoryMock.Received(1).GetByIdAsync(Arg.Any<BrandId>(), Arg.Any<CancellationToken>());

        await _brandRepositoryMock.Received(1).NameExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());

        await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
