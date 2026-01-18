using NSubstitute;
using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Features.BrandUseCases.CreateBrand;
using Overclocked.Domain.BrandAggregate;
using Overclocked.SharedKernel;
using Shouldly;

namespace Overclocked.Unit.Tests.BrandTests;

public class CreateBrandRequestHandlerTest
{
    private readonly IBrandRepository _brandRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly CreateBrandRequestHandler _createBrandRequestHandler;

    public CreateBrandRequestHandlerTest()
    {
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _brandRepositoryMock = Substitute.For<IBrandRepository>();

        _createBrandRequestHandler = new CreateBrandRequestHandler(_brandRepositoryMock, _unitOfWorkMock);
    }

    [Fact]
    public async Task CreateBrandRequestHandler_Should_ReturnFailure_When_NameAlreadyExists()
    {
        // Arrange
        var request = new CreateBrandRequest
        {
            Name = "Brand Name",
            ImageUrl = "https://res.cloudinary.com/over-clocked/image.png",
        };

        _brandRepositoryMock.NameExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(true);

        // Act
        Result result = await _createBrandRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);

        await _brandRepositoryMock.Received(1).NameExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateBrandRequestHandler_Should_ReturnFailure_When_ImageUrlIsInvalid()
    {
        // Arrange
        var request = new CreateBrandRequest
        {
            Name = "Brand Name",
            ImageUrl = "https://res.cloudinary.com/over-clocked/image.txt",
        };

        _brandRepositoryMock.NameExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(false);

        // Act
        Result result = await _createBrandRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);

        await _brandRepositoryMock.Received(1).NameExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateBrandRequestHandler_Should_ReturnFailure_When_NameIsInvalid()
    {
        // Arrange
        var request = new CreateBrandRequest { Name = "  ", ImageUrl = "https://res.cloudinary.com/over-clocked/image.txt" };

        _brandRepositoryMock.NameExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(false);

        // Act
        Result result = await _createBrandRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);

        await _brandRepositoryMock.Received(1).NameExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateBrandRequestHandler_Should_ReturnSuccess_When_ThereIsNoError()
    {
        // Arrange
        var request = new CreateBrandRequest
        {
            Name = "Brand Name",
            ImageUrl = "https://res.cloudinary.com/over-clocked/image.png",
        };

        _brandRepositoryMock.NameExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(false);

        _unitOfWorkMock.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        Result result = await _createBrandRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBe(Error.None);

        await _brandRepositoryMock.Received(1).NameExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());

        _brandRepositoryMock.Received(1).Add(Arg.Any<Brand>());

        await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
