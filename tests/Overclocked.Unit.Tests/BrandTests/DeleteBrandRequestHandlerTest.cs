using NSubstitute;
using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Features.BrandUseCases.DeleteBrand;
using Overclocked.Architecture.Tests.FakeData;
using Overclocked.Domain.BrandAggregate;
using Overclocked.Domain.BrandAggregate.ValueObjects;
using Overclocked.SharedKernel;
using Shouldly;

namespace Overclocked.Unit.Tests.BrandTests;

public class DeleteBrandRequestHandlerTest
{
    private readonly IBrandRepository _brandRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly DeleteBrandRequestHandler _deleteBrandRequestHandler;

    public DeleteBrandRequestHandlerTest()
    {
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _brandRepositoryMock = Substitute.For<IBrandRepository>();

        _deleteBrandRequestHandler = new DeleteBrandRequestHandler(_brandRepositoryMock, _unitOfWorkMock);
    }

    [Fact]
    public async Task DeleteBrandRequestHandler_Should_ReturnFailure_When_BrandDoesNotExists()
    {
        // Arrange
        var brandId = Guid.CreateVersion7();
        var request = new DeleteBrandRequest { Id = brandId };

        _brandRepositoryMock.GetByIdAsync(Arg.Any<BrandId>(), Arg.Any<CancellationToken>()).Returns((Brand)null!);

        // Act
        Result result = await _deleteBrandRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);
        result.Error.Type.ShouldBe(ErrorType.NotFound);

        await _brandRepositoryMock.Received(1).GetByIdAsync(Arg.Any<BrandId>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteBrandRequestHandler_Should_ReturnSuccess_When_BrandExists()
    {
        // Arrange
        var brandId = Guid.CreateVersion7();
        var request = new DeleteBrandRequest { Id = brandId };

        Brand brand = new BrandFaker().Generate();

        _brandRepositoryMock.GetByIdAsync(Arg.Any<BrandId>(), Arg.Any<CancellationToken>()).Returns(brand);

        _brandRepositoryMock.Remove(Arg.Any<Brand>());

        _unitOfWorkMock.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        Result result = await _deleteBrandRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBe(Error.None);
        brand.DomainEvents.ShouldNotBeEmpty();

        await _brandRepositoryMock.Received(1).GetByIdAsync(Arg.Any<BrandId>(), Arg.Any<CancellationToken>());

        await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());

        _brandRepositoryMock.Received(1).Remove(Arg.Any<Brand>());
    }
}
