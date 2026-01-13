using NSubstitute;
using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Features.ProductUseCases.CreateProduct;
using Overclocked.Application.Features.ProductUseCases.DTOs.Responses;
using Overclocked.Architecture.Tests.FakeData;
using Overclocked.Domain.BrandAggregate.ValueObjects;
using Overclocked.Domain.CategoryAggregate.ValueObjects;
using Overclocked.Domain.ProductAggregate;
using Overclocked.SharedKernel;
using Shouldly;

namespace Overclocked.Unit.Tests.ProductTests;

public class CreateProductRequestHandlerTest
{
    private readonly IProductRepository _productRepositoryMock;
    private readonly IBrandRepository _brandRepositoryMock;
    private readonly ICategoryRepository _categoryRepositoryMock;
    private readonly ITagReadRepository _tagReadRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly CreateProductRequestHandler _createProductRequestHandler;

    public CreateProductRequestHandlerTest()
    {
        _productRepositoryMock = Substitute.For<IProductRepository>();
        _brandRepositoryMock = Substitute.For<IBrandRepository>();
        _categoryRepositoryMock = Substitute.For<ICategoryRepository>();
        _tagReadRepositoryMock = Substitute.For<ITagReadRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();

        _createProductRequestHandler = new CreateProductRequestHandler(
            _productRepositoryMock,
            _brandRepositoryMock,
            _categoryRepositoryMock,
            _tagReadRepositoryMock,
            _unitOfWorkMock
        );
    }

    [Fact]
    public async Task CreateProductRequestHandler_Should_ReturnFailure_When_NameAlreadyExists()
    {
        // Arrange
        var request = new CreateProductRequest
        {
            BrandId = Guid.NewGuid(),
            CategoryId = Guid.NewGuid(),
            Name = "Product Name",
            Thumbnail = "https://res.cloudinary.com/over-clocked/brands/image.jpg",
            Description = "Description",
            Price = 7200,
            StockQuantity = 45,
            Discount = 0.0m,
            Tags = [Guid.NewGuid()],
            Images = null,
            Specifications = [new ProductSpecificationDto { Name = "Specification Name", Value = "Specification Value" }],
        };

        _productRepositoryMock.NameExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(true);

        // Act
        Result result = await _createProductRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);

        await _productRepositoryMock.Received(1).NameExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateProductRequestHandler_Should_ReturnFailure_When_BrandDoesNotExist()
    {
        // Arrange
        var request = new CreateProductRequest
        {
            BrandId = Guid.NewGuid(),
            CategoryId = Guid.NewGuid(),
            Name = "Product Name",
            Thumbnail = "https://res.cloudinary.com/over-clocked/brands/image.jpg",
            Description = "Description",
            Price = 7200,
            StockQuantity = 45,
            Discount = 0.0m,
            Tags = [Guid.NewGuid()],
            Images = null,
            Specifications = [new ProductSpecificationDto { Name = "Specification Name", Value = "Specification Value" }],
        };

        _productRepositoryMock.NameExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(false);

        _brandRepositoryMock.ExistsAsync(Arg.Any<BrandId>(), Arg.Any<CancellationToken>()).Returns(false);

        // Act
        Result result = await _createProductRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);

        await _productRepositoryMock.Received(1).NameExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());

        await _brandRepositoryMock.Received(1).ExistsAsync(Arg.Any<BrandId>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateProductRequestHandler_Should_ReturnFailure_When_CategoryDoesNotExist()
    {
        // Arrange
        var request = new CreateProductRequest
        {
            BrandId = Guid.NewGuid(),
            CategoryId = Guid.NewGuid(),
            Name = "Product Name",
            Thumbnail = "https://res.cloudinary.com/over-clocked/brands/image.jpg",
            Description = "Description",
            Price = 7200,
            StockQuantity = 45,
            Discount = 0.0m,
            Tags = [Guid.NewGuid()],
            Images = null,
            Specifications = [new ProductSpecificationDto { Name = "Specification Name", Value = "Specification Value" }],
        };

        _productRepositoryMock.NameExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(false);

        _brandRepositoryMock.ExistsAsync(Arg.Any<BrandId>(), Arg.Any<CancellationToken>()).Returns(true);

        _categoryRepositoryMock.ExistsAsync(Arg.Any<CategoryId>(), Arg.Any<CancellationToken>()).Returns(false);

        // Act
        Result result = await _createProductRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);

        await _productRepositoryMock.Received(1).NameExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());

        await _brandRepositoryMock.Received(1).ExistsAsync(Arg.Any<BrandId>(), Arg.Any<CancellationToken>());

        await _categoryRepositoryMock.Received(1).ExistsAsync(Arg.Any<CategoryId>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateProductRequestHandler_Should_ReturnFailure_When_SomeTagsDoesNotExist()
    {
        // Arrange
        List<Guid> tags = [Guid.NewGuid(), Guid.NewGuid()];
        var request = new CreateProductRequest
        {
            BrandId = Guid.NewGuid(),
            CategoryId = Guid.NewGuid(),
            Name = "Product Name",
            Thumbnail = "https://res.cloudinary.com/over-clocked/brands/image.jpg",
            Description = "Description",
            Price = 7200,
            StockQuantity = 45,
            Discount = 0.0m,
            Tags = tags.Append(Guid.NewGuid()).ToList(),
            Images = null,
            Specifications = [new ProductSpecificationDto { Name = "Specification Name", Value = "Specification Value" }],
        };

        _productRepositoryMock.NameExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(false);

        _brandRepositoryMock.ExistsAsync(Arg.Any<BrandId>(), Arg.Any<CancellationToken>()).Returns(true);

        _categoryRepositoryMock.ExistsAsync(Arg.Any<CategoryId>(), Arg.Any<CancellationToken>()).Returns(true);

        _tagReadRepositoryMock.GetExistingTagIdsAsync(Arg.Any<List<Guid>>(), Arg.Any<CancellationToken>()).Returns(tags);

        // Act
        Result result = await _createProductRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);

        await _productRepositoryMock.Received(1).NameExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());

        await _brandRepositoryMock.Received(1).ExistsAsync(Arg.Any<BrandId>(), Arg.Any<CancellationToken>());

        await _categoryRepositoryMock.Received(1).ExistsAsync(Arg.Any<CategoryId>(), Arg.Any<CancellationToken>());

        await _tagReadRepositoryMock.Received(1).GetExistingTagIdsAsync(Arg.Any<List<Guid>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateProductRequestHandler_Should_ReturnFailure_When_ThumbnailIsInvalid()
    {
        // Arrange
        List<Guid> tags = [Guid.NewGuid(), Guid.NewGuid()];
        var request = new CreateProductRequest
        {
            BrandId = Guid.NewGuid(),
            CategoryId = Guid.NewGuid(),
            Name = "Product Name",
            Thumbnail = "ftp://res.cloudinary.com/over-clocked/brands/image.jpg",
            Description = "Description",
            Price = 7200,
            StockQuantity = 45,
            Discount = 0.0m,
            Tags = tags,
            Images = null,
            Specifications = [new ProductSpecificationDto { Name = "Specification Name", Value = "Specification Value" }],
        };

        _productRepositoryMock.NameExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(false);

        _brandRepositoryMock.ExistsAsync(Arg.Any<BrandId>(), Arg.Any<CancellationToken>()).Returns(true);

        _categoryRepositoryMock.ExistsAsync(Arg.Any<CategoryId>(), Arg.Any<CancellationToken>()).Returns(true);

        _tagReadRepositoryMock.GetExistingTagIdsAsync(Arg.Any<List<Guid>>(), Arg.Any<CancellationToken>()).Returns(tags);

        // Act
        Result result = await _createProductRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);

        await _productRepositoryMock.Received(1).NameExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());

        await _brandRepositoryMock.Received(1).ExistsAsync(Arg.Any<BrandId>(), Arg.Any<CancellationToken>());

        await _categoryRepositoryMock.Received(1).ExistsAsync(Arg.Any<CategoryId>(), Arg.Any<CancellationToken>());

        await _tagReadRepositoryMock.Received(1).GetExistingTagIdsAsync(Arg.Any<List<Guid>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateProductRequestHandler_Should_ReturnFailure_When_PriceIsInvalid()
    {
        // Arrange
        List<Guid> tags = [Guid.NewGuid(), Guid.NewGuid()];
        var request = new CreateProductRequest
        {
            BrandId = Guid.NewGuid(),
            CategoryId = Guid.NewGuid(),
            Name = "Product Name",
            Thumbnail = "https://res.cloudinary.com/over-clocked/brands/image.jpg",
            Description = "Description",
            Price = -10,
            StockQuantity = 45,
            Discount = 0.0m,
            Tags = tags,
            Images = null,
            Specifications = [new ProductSpecificationDto { Name = "Specification Name", Value = "Specification Value" }],
        };

        _productRepositoryMock.NameExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(false);

        _brandRepositoryMock.ExistsAsync(Arg.Any<BrandId>(), Arg.Any<CancellationToken>()).Returns(true);

        _categoryRepositoryMock.ExistsAsync(Arg.Any<CategoryId>(), Arg.Any<CancellationToken>()).Returns(true);

        _tagReadRepositoryMock.GetExistingTagIdsAsync(Arg.Any<List<Guid>>(), Arg.Any<CancellationToken>()).Returns(tags);

        // Act
        Result result = await _createProductRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);

        await _productRepositoryMock.Received(1).NameExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());

        await _brandRepositoryMock.Received(1).ExistsAsync(Arg.Any<BrandId>(), Arg.Any<CancellationToken>());

        await _categoryRepositoryMock.Received(1).ExistsAsync(Arg.Any<CategoryId>(), Arg.Any<CancellationToken>());

        await _tagReadRepositoryMock.Received(1).GetExistingTagIdsAsync(Arg.Any<List<Guid>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateProductRequestHandler_Should_ReturnFailure_When_DiscountIsInvalid()
    {
        // Arrange
        List<Guid> tags = [Guid.NewGuid(), Guid.NewGuid()];
        var request = new CreateProductRequest
        {
            BrandId = Guid.NewGuid(),
            CategoryId = Guid.NewGuid(),
            Name = "Product Name",
            Thumbnail = "https://res.cloudinary.com/over-clocked/brands/image.jpg",
            Description = "Description",
            Price = 7200,
            StockQuantity = 45,
            Discount = 10m,
            Tags = tags,
            Images = null,
            Specifications = [new ProductSpecificationDto { Name = "Specification Name", Value = "Specification Value" }],
        };

        _productRepositoryMock.NameExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(false);

        _brandRepositoryMock.ExistsAsync(Arg.Any<BrandId>(), Arg.Any<CancellationToken>()).Returns(true);

        _categoryRepositoryMock.ExistsAsync(Arg.Any<CategoryId>(), Arg.Any<CancellationToken>()).Returns(true);

        _tagReadRepositoryMock.GetExistingTagIdsAsync(Arg.Any<List<Guid>>(), Arg.Any<CancellationToken>()).Returns(tags);

        // Act
        Result result = await _createProductRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);

        await _productRepositoryMock.Received(1).NameExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());

        await _brandRepositoryMock.Received(1).ExistsAsync(Arg.Any<BrandId>(), Arg.Any<CancellationToken>());

        await _categoryRepositoryMock.Received(1).ExistsAsync(Arg.Any<CategoryId>(), Arg.Any<CancellationToken>());

        await _tagReadRepositoryMock.Received(1).GetExistingTagIdsAsync(Arg.Any<List<Guid>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateProductRequestHandler_Should_ReturnFailure_When_SpecificationsIsInvalid()
    {
        // Arrange
        List<Guid> tags = [Guid.NewGuid(), Guid.NewGuid()];
        var request = new CreateProductRequest
        {
            BrandId = Guid.NewGuid(),
            CategoryId = Guid.NewGuid(),
            Name = "Product Name",
            Thumbnail = "https://res.cloudinary.com/over-clocked/brands/image.jpg",
            Description = "Description",
            Price = 7200,
            StockQuantity = 45,
            Discount = 10m,
            Tags = tags,
            Images = null,
            Specifications = [new ProductSpecificationDto { Name = "Specification Name", Value = "   " }],
        };

        _productRepositoryMock.NameExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(false);

        _brandRepositoryMock.ExistsAsync(Arg.Any<BrandId>(), Arg.Any<CancellationToken>()).Returns(true);

        _categoryRepositoryMock.ExistsAsync(Arg.Any<CategoryId>(), Arg.Any<CancellationToken>()).Returns(true);

        _tagReadRepositoryMock.GetExistingTagIdsAsync(Arg.Any<List<Guid>>(), Arg.Any<CancellationToken>()).Returns(tags);

        // Act
        Result result = await _createProductRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);

        await _productRepositoryMock.Received(1).NameExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());

        await _brandRepositoryMock.Received(1).ExistsAsync(Arg.Any<BrandId>(), Arg.Any<CancellationToken>());

        await _categoryRepositoryMock.Received(1).ExistsAsync(Arg.Any<CategoryId>(), Arg.Any<CancellationToken>());

        await _tagReadRepositoryMock.Received(1).GetExistingTagIdsAsync(Arg.Any<List<Guid>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateProductRequestHandler_Should_ReturnFailure_When_ImageIsInvalid()
    {
        // Arrange
        List<Guid> tags = [Guid.NewGuid(), Guid.NewGuid()];
        var request = new CreateProductRequest
        {
            BrandId = Guid.NewGuid(),
            CategoryId = Guid.NewGuid(),
            Name = "Product Name",
            Thumbnail = "https://res.cloudinary.com/over-clocked/brands/image.jpg",
            Description = "Description",
            Price = 7200,
            StockQuantity = 45,
            Discount = 10m,
            Tags = tags,
            Images = ["ftp://res.cloudinary.com/over-clocked/brands/image.jpg"],
            Specifications = [new ProductSpecificationDto { Name = "Specification Name", Value = "Specification Value" }],
        };

        _productRepositoryMock.NameExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(false);

        _brandRepositoryMock.ExistsAsync(Arg.Any<BrandId>(), Arg.Any<CancellationToken>()).Returns(true);

        _categoryRepositoryMock.ExistsAsync(Arg.Any<CategoryId>(), Arg.Any<CancellationToken>()).Returns(true);

        _tagReadRepositoryMock.GetExistingTagIdsAsync(Arg.Any<List<Guid>>(), Arg.Any<CancellationToken>()).Returns(tags);

        // Act
        Result result = await _createProductRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);

        await _productRepositoryMock.Received(1).NameExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());

        await _brandRepositoryMock.Received(1).ExistsAsync(Arg.Any<BrandId>(), Arg.Any<CancellationToken>());

        await _categoryRepositoryMock.Received(1).ExistsAsync(Arg.Any<CategoryId>(), Arg.Any<CancellationToken>());

        await _tagReadRepositoryMock.Received(1).GetExistingTagIdsAsync(Arg.Any<List<Guid>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateProductRequestHandler_Should_ReturnFailure_When_TagIsInvalid()
    {
        // Arrange
        var tagId = Guid.NewGuid();
        List<Guid> tags = [tagId, tagId];
        var request = new CreateProductRequest
        {
            BrandId = Guid.NewGuid(),
            CategoryId = Guid.NewGuid(),
            Name = "Product Name",
            Thumbnail = "https://res.cloudinary.com/over-clocked/brands/image.jpg",
            Description = "Description",
            Price = 7200,
            StockQuantity = 45,
            Discount = 10m,
            Tags = tags,
            Images = null,
            Specifications = [new ProductSpecificationDto { Name = "Specification Name", Value = "Specification Value" }],
        };

        _productRepositoryMock.NameExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(false);

        _brandRepositoryMock.ExistsAsync(Arg.Any<BrandId>(), Arg.Any<CancellationToken>()).Returns(true);

        _categoryRepositoryMock.ExistsAsync(Arg.Any<CategoryId>(), Arg.Any<CancellationToken>()).Returns(true);

        _tagReadRepositoryMock.GetExistingTagIdsAsync(Arg.Any<List<Guid>>(), Arg.Any<CancellationToken>()).Returns(tags);

        // Act
        Result result = await _createProductRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);

        await _productRepositoryMock.Received(1).NameExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());

        await _brandRepositoryMock.Received(1).ExistsAsync(Arg.Any<BrandId>(), Arg.Any<CancellationToken>());

        await _categoryRepositoryMock.Received(1).ExistsAsync(Arg.Any<CategoryId>(), Arg.Any<CancellationToken>());

        await _tagReadRepositoryMock.Received(1).GetExistingTagIdsAsync(Arg.Any<List<Guid>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateProductRequestHandler_Should_ReturnFailure_When_NameIsInvalid()
    {
        // Arrange
        var tagId = Guid.NewGuid();
        List<Guid> tags = [tagId, tagId];
        var request = new CreateProductRequest
        {
            BrandId = Guid.NewGuid(),
            CategoryId = Guid.NewGuid(),
            Name = "           ",
            Thumbnail = "https://res.cloudinary.com/over-clocked/brands/image.jpg",
            Description = "Description",
            Price = 7200,
            StockQuantity = 45,
            Discount = 10m,
            Tags = tags,
            Images = null,
            Specifications = [new ProductSpecificationDto { Name = "Specification Name", Value = "Specification Value" }],
        };

        _productRepositoryMock.NameExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(false);

        _brandRepositoryMock.ExistsAsync(Arg.Any<BrandId>(), Arg.Any<CancellationToken>()).Returns(true);

        _categoryRepositoryMock.ExistsAsync(Arg.Any<CategoryId>(), Arg.Any<CancellationToken>()).Returns(true);

        _tagReadRepositoryMock.GetExistingTagIdsAsync(Arg.Any<List<Guid>>(), Arg.Any<CancellationToken>()).Returns(tags);

        // Act
        Result result = await _createProductRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);

        await _productRepositoryMock.Received(1).NameExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());

        await _brandRepositoryMock.Received(1).ExistsAsync(Arg.Any<BrandId>(), Arg.Any<CancellationToken>());

        await _categoryRepositoryMock.Received(1).ExistsAsync(Arg.Any<CategoryId>(), Arg.Any<CancellationToken>());

        await _tagReadRepositoryMock.Received(1).GetExistingTagIdsAsync(Arg.Any<List<Guid>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateProductRequestHandler_Should_ReturnSuccess_When_ThereIsNoError()
    {
        // Arrange
        List<Guid> tags = [Guid.NewGuid(), Guid.NewGuid()];
        var request = new CreateProductRequest
        {
            BrandId = Guid.NewGuid(),
            CategoryId = Guid.NewGuid(),
            Name = "Product Name",
            Thumbnail = "https://res.cloudinary.com/over-clocked/brands/image.jpg",
            Description = "Description",
            Price = 7200,
            StockQuantity = 45,
            Discount = 0.0m,
            Tags = tags,
            Images = null,
            Specifications = [new ProductSpecificationDto { Name = "Specification Name", Value = "Specification Value" }],
        };

        _productRepositoryMock.NameExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(false);

        _brandRepositoryMock.ExistsAsync(Arg.Any<BrandId>(), Arg.Any<CancellationToken>()).Returns(true);

        _categoryRepositoryMock.ExistsAsync(Arg.Any<CategoryId>(), Arg.Any<CancellationToken>()).Returns(true);

        _tagReadRepositoryMock.GetExistingTagIdsAsync(Arg.Any<List<Guid>>(), Arg.Any<CancellationToken>()).Returns(tags);

        Product product = new ProductFaker(Guid.NewGuid(), Guid.NewGuid()).Generate();

        _productRepositoryMock.Add(Arg.Any<Product>());
        _unitOfWorkMock.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        Result result = await _createProductRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();

        _productRepositoryMock.Received(1).Add(Arg.Any<Product>());

        await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
