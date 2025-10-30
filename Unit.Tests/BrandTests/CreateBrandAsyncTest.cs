using System.Net;
using ArchitectureTests.FakeData;
using NSubstitute;
using Shouldly;
using Domain.Entities;
using Application.Abstraction.Repositories;
using Application.Abstraction.Services;
using Application.Services.Brand;
using Application.Services.Brand.DTOs.Request;
using Application.Services;

namespace Unit.Tests.BrandTests;

public class CreateBrandAsyncTest
{
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly IBrandRepository _brandRepositoryMock;
    private readonly IBrandService _brandServices;
    private readonly IFileStorageService _fileStorageServiceMock;

    public CreateBrandAsyncTest()
    {
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _brandRepositoryMock = Substitute.For<IBrandRepository>();
        _fileStorageServiceMock = Substitute.For<IFileStorageService>();
        _brandServices = new BrandService(_brandRepositoryMock, _unitOfWorkMock, _fileStorageServiceMock);
    }

    [Fact]
    public async Task Handle_WhenThereIsNoError_ShouldReturnSuccess()
    {
        // Arrange
        var request = new CreateBrandRequest
        {
            Name = "Nike",
            ImageUrl = "image.png"
        };

        var brand = new BrandFaker().Generate();

        _brandRepositoryMock.AddAsync(Arg.Any<Brand>(), Arg.Any<CancellationToken>())
            .Returns(brand);

        _unitOfWorkMock.CompleteAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(1));

        // Act
        var result = await _brandServices.CreateBrandAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.StatusCode.ShouldBe(HttpStatusCode.Created);

        await _brandRepositoryMock.Received(1)
            .AddAsync(Arg.Any<Brand>(), Arg.Any<CancellationToken>());

        await _unitOfWorkMock.Received(1)
            .CompleteAsync(Arg.Any<CancellationToken>());
    }
}
