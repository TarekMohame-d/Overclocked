using System.Linq.Expressions;
using Application.Abstraction.Services;
using Application.Features.Brand.Commands.CreateBrand.Notifications;
using ArchitectureTests.FakeData;
using Domain.Repositories;
using Hangfire;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using Shouldly;

namespace Unit.Tests.BrandTests.Commands.CreateBrand.Notifications;

public class BrandCreatedUploadImageHandlerTest
{
    private readonly IFileStorageService _fileStorageServiceMock;
    private readonly IBrandRepository _brandRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly IBackgroundJobClientWrapper _backgroundJobClientMock;
    private readonly BrandCreatedUploadImageHandler _handler;

    public BrandCreatedUploadImageHandlerTest()
    {
        _fileStorageServiceMock = Substitute.For<IFileStorageService>();
        _brandRepositoryMock = Substitute.For<IBrandRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _backgroundJobClientMock = Substitute.For<IBackgroundJobClientWrapper>();

        _handler = new BrandCreatedUploadImageHandler(
            _fileStorageServiceMock,
            _brandRepositoryMock,
            _unitOfWorkMock,
            _backgroundJobClientMock);
    }

    [Fact]
    public async Task Handle_Should_Enqueue_BackgroundJob()
    {
        // Arrange
        var fileMock = Substitute.For<IFormFile>();
        fileMock.FileName.Returns("logo.png");

        _backgroundJobClientMock.Enqueue(Arg.Any<Expression<Func<Task>>>())
            .Returns("jobId");

        var notification = new BrandCreatedNotification(Guid.NewGuid(), fileMock);

        // Act
        await _handler.Handle(notification, CancellationToken.None);

        // Assert
        _backgroundJobClientMock.Received(1)
            .Enqueue(Arg.Any<Expression<Func<Task>>>());
    }

    [Fact]
    public async Task UploadAndUpdateBrandImageAsync_Should_Update_Brand_And_DeleteTempFile()
    {
        // Arrange
        var filePath = Path.GetTempFileName();
        await File.WriteAllTextAsync(filePath, "fakecontent");

        var fileName = "logo.png";

        var brand = new BrandFaker().Generate();

        _fileStorageServiceMock.UploadFileAsync(Arg.Any<Stream>(), fileName, "brands", Arg.Any<CancellationToken>())
            .Returns("http://cdn.com/logo.png");

        _brandRepositoryMock.GetByIdAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(brand);

        // Act
        await _handler.UploadAndUpdateBrandImageAsync(brand.Id, filePath, fileName);

        // Assert
        brand.Image.ShouldBe("http://cdn.com/logo.png");

        _brandRepositoryMock.Received(1).Update(brand);
        await _unitOfWorkMock.Received(1).CompleteAsync(Arg.Any<CancellationToken>());

        File.Exists(filePath).ShouldBeFalse(); // cleaned up
    }
}
