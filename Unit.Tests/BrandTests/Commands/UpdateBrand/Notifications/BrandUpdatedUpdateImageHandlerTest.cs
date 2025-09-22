using System.Linq.Expressions;
using Application.Abstraction.Services;
using Application.Features.Brand.Commands.UpdateBrand.Notifications;
using ArchitectureTests.FakeData;
using Domain.Repositories;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using Shouldly;

namespace Unit.Tests.BrandTests.Commands.UpdateBrand.Notifications;

public class BrandUpdatedUpdateImageHandlerTest
{
    private readonly IFileStorageService _fileStorageServiceMock;
    private readonly IBrandRepository _brandRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly IBackgroundJobClientWrapper _backgroundJobClientMock;
    private readonly BrandUpdatedUpdateImageHandler _handler;

    public BrandUpdatedUpdateImageHandlerTest()
    {
        _fileStorageServiceMock = Substitute.For<IFileStorageService>();
        _brandRepositoryMock = Substitute.For<IBrandRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _backgroundJobClientMock = Substitute.For<IBackgroundJobClientWrapper>();

        _handler = new BrandUpdatedUpdateImageHandler(
            _fileStorageServiceMock,
            _brandRepositoryMock,
            _unitOfWorkMock,
            _backgroundJobClientMock);
    }

    [Fact]
    public async Task Handle_WhenImageProvided_ShouldEnqueueUploadAndDeleteJobs()
    {
        // Arrange
        var fileMock = Substitute.For<IFormFile>();
        fileMock.FileName.Returns("newlogo.png");

        _backgroundJobClientMock.Enqueue(Arg.Any<Expression<Func<Task>>>())
            .Returns("uploadJobId");

        _backgroundJobClientMock.ContinueJobWith("uploadJobId", Arg.Any<Expression<Func<Task>>>())
            .Returns("deleteJobId");

        var notification = new BrandUpdatedNotification(
            Guid.NewGuid(),
            fileMock,
            "http://cdn.com/oldlogo.png"
        );

        // Act
        await _handler.Handle(notification, CancellationToken.None);

        // Assert
        _backgroundJobClientMock.Received(1)
            .Enqueue(Arg.Any<Expression<Func<Task>>>());

        _backgroundJobClientMock.Received(1)
            .ContinueJobWith("uploadJobId", Arg.Any<Expression<Func<Task>>>());
    }

    [Fact]
    public async Task Handle_WhenImageNotProvided_ShouldDoNothing()
    {
        // Arrange
        var notification = new BrandUpdatedNotification(Guid.NewGuid(), null, "http://cdn.com/oldlogo.png");

        // Act
        await _handler.Handle(notification, CancellationToken.None);

        // Assert
        _backgroundJobClientMock.DidNotReceive()
            .Enqueue(Arg.Any<Expression<Func<Task>>>());
        _backgroundJobClientMock.DidNotReceive()
            .ContinueJobWith(Arg.Any<string>(), Arg.Any<Expression<Func<Task>>>());
    }

    [Fact]
    public async Task UploadAndUpdateBrandImageAsync_ShouldUpdateBrandAndDeleteTempFile()
    {
        // Arrange
        var filePath = Path.GetTempFileName();
        await File.WriteAllTextAsync(filePath, "fakecontent");

        var fileName = "updated.png";
        var brand = new BrandFaker().Generate();

        _fileStorageServiceMock.UploadFileAsync(Arg.Any<Stream>(), fileName, "brands", Arg.Any<CancellationToken>())
            .Returns("http://cdn.com/updated.png");

        _brandRepositoryMock.GetByIdAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(brand);

        // Act
        await _handler.UploadAndUpdateBrandImageAsync(brand.Id, filePath, fileName);

        // Assert
        brand.Image.ShouldBe("http://cdn.com/updated.png");

        _brandRepositoryMock.Received(1).Update(brand);
        await _unitOfWorkMock.Received(1).CompleteAsync(Arg.Any<CancellationToken>());

        File.Exists(filePath).ShouldBeFalse(); // cleaned up
    }

    [Fact]
    public async Task DeleteBrandImageAsync_ShouldCallFileStorageDelete()
    {
        // Arrange
        var imageUrl = "http://cdn.com/old.png";

        _fileStorageServiceMock.DeleteFileAsync(imageUrl, Arg.Any<CancellationToken>())
            .Returns(true);

        // Act
        await _handler.DeleteBrandImageAsync(imageUrl);

        // Assert
        await _fileStorageServiceMock.Received(1)
            .DeleteFileAsync(imageUrl, Arg.Any<CancellationToken>());
    }
}
