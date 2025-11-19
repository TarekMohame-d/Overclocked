using System.Net;
using Application.Abstraction.Messaging;
using Application.Abstraction.Repositories;
using Application.Common.Results;
using Application.Services.Brand;
using Application.Services.Brand.DTOs.Request;
using ArchitectureTests.FakeData;
using Domain.Entities;
using NSubstitute;
using Shouldly;

namespace Unit.Tests.BrandTests;

public class CreateBrandAsyncTest
{
    private readonly IBrandRepository _brandRepositoryMock;
    private readonly BrandService _brandServices;
    private readonly IUnitOfWork _unitOfWorkMock;

    public CreateBrandAsyncTest()
    {
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _brandRepositoryMock = Substitute.For<IBrandRepository>();
        _brandServices = new BrandService(_brandRepositoryMock, _unitOfWorkMock, Substitute.For<IEventDispatcher>());
    }

    [Fact]
    public async Task CreateBrandAsync_When_ThereIsNoError_Should_ReturnSuccess()
    {
        // Arrange
        var request = new CreateBrandRequest { Name = "Brand Name", ImageUrl = "image.png" };

        Brand brand = new BrandFaker().Generate();

        _brandRepositoryMock.AddAsync(Arg.Any<Brand>(), Arg.Any<CancellationToken>())
            .Returns(brand);

        _unitOfWorkMock.CompleteAsync(Arg.Any<CancellationToken>())
            .Returns(1);

        // Act
        Result result = await _brandServices.CreateBrandAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.StatusCode.ShouldBe(HttpStatusCode.Created);

        await _brandRepositoryMock.Received(1).AddAsync(Arg.Any<Brand>(), Arg.Any<CancellationToken>());

        await _unitOfWorkMock.Received(1).CompleteAsync(Arg.Any<CancellationToken>());
    }
}
