using System.Net;
using NSubstitute;
using Overclocked.Application.Abstraction;
using Overclocked.Application.Abstraction.Persistence;
using Overclocked.Application.Brand.Commands;
using Overclocked.Application.Brand.Commands.CreateBrand;
using Overclocked.Architecture.Tests.FakeData;
using Overclocked.Domain.BrandAggregate;
using Overclocked.Domain.Common.Results;
using Shouldly;

namespace Overclocked.Unit.Tests.BrandTests;

public class CreateBrandCommandHandlerTest
{
    private readonly IBrandRepository _brandRepositoryMock;
    private readonly IBrandCommands _brandCommands;
    private readonly IUnitOfWork _unitOfWorkMock;

    public CreateBrandCommandHandlerTest()
    {
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _brandRepositoryMock = Substitute.For<IBrandRepository>();

        _brandCommands = new BrandCommands(_brandRepositoryMock, _unitOfWorkMock);
    }

    [Fact]
    public async Task CreateBrandCommandHandler_Should_ReturnSuccess_When_ThereIsNoError()
    {
        // Arrange
        var command = new CreateBrandCommand
        {
            Name = "Brand Name",
            ImageUrl = "https://res.cloudinary.com/over-clocked/image.png"
        };

        Brand brand = new BrandFaker().Generate();

        _brandRepositoryMock.AddAsync(Arg.Any<Brand>(), Arg.Any<CancellationToken>())
            .Returns(brand);

        _unitOfWorkMock.SaveChangesAsync(Arg.Any<CancellationToken>())
            .Returns(1);

        // Act
        Result result = await _brandCommands.CreateBrandCommandHandler(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.StatusCode.ShouldBe(HttpStatusCode.Created);
        result.Error.ShouldBe(Error.None);

        await _brandRepositoryMock.Received(1)
            .AddAsync(Arg.Any<Brand>(), Arg.Any<CancellationToken>());

        await _unitOfWorkMock.Received(1)
            .SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
