using System.Linq.Expressions;
using NSubstitute;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Features.BrandUseCases.DTOs.Responses;
using Overclocked.Application.Features.CategoryUseCases.DTOs.Responses;
using Overclocked.Application.Features.ProductUseCases.DTOs.Responses;
using Overclocked.Application.Features.ProductUseCases.GetProductById;
using Overclocked.Domain.ProductAggregate;
using Overclocked.Domain.ProductAggregate.ValueObjects;
using Overclocked.SharedKernel;
using Shouldly;

namespace Overclocked.Unit.Tests.ProductTests;

public class GetProductByIdRequestHandlerTest
{
    private readonly IProductReadRepository _productReadRepositoryMock;
    private readonly IReviewReadRepository _reviewReadRepositoryMock;
    private readonly GetProductByIdRequestHandler _getProductByIdRequestHandler;

    public GetProductByIdRequestHandlerTest()
    {
        _productReadRepositoryMock = Substitute.For<IProductReadRepository>();
        _reviewReadRepositoryMock = Substitute.For<IReviewReadRepository>();

        _getProductByIdRequestHandler = new GetProductByIdRequestHandler(_productReadRepositoryMock, _reviewReadRepositoryMock);
    }

    [Fact]
    public async Task GetProductByIdRequestHandler_Should_ReturnFailure_When_ProductDoesNotExist()
    {
        // Arrange
        _productReadRepositoryMock
            .GetByIdAsync(
                Arg.Any<ProductId>(),
                Arg.Any<Expression<Func<Product, ProductResponse>>>(),
                Arg.Any<CancellationToken>()
            )
            .Returns((ProductResponse)null!);

        var request = new GetProductByIdRequest { Id = Guid.NewGuid() };

        // Act
        Result<ProductResponse> result = await _getProductByIdRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);

        await _productReadRepositoryMock
            .Received(1)
            .GetByIdAsync(
                Arg.Any<ProductId>(),
                Arg.Any<Expression<Func<Product, ProductResponse>>>(),
                Arg.Any<CancellationToken>()
            );

        await _reviewReadRepositoryMock
            .DidNotReceive()
            .GetProductRatingsBreakdownAsync(Arg.Any<ProductId>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetProductByIdRequestHandler_Should_ReturnSuccess_When_ProductExists()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var request = new GetProductByIdRequest { Id = productId };

        _productReadRepositoryMock
            .GetByIdAsync(
                Arg.Any<ProductId>(),
                Arg.Any<Expression<Func<Product, ProductResponse>>>(),
                Arg.Any<CancellationToken>()
            )
            .Returns(
                new ProductResponse
                {
                    Id = productId,
                    Name = "Product Name",
                    Thumbnail = "https://res.cloudinary.com/over-clocked/products/image.jpg",
                    Description = "Description",
                    Price = 7200,
                    Discount = 0.0m,
                    Rating = 4.5,
                    ReviewCount = 10,
                    Brand = new BrandResponse
                    {
                        Id = Guid.NewGuid(),
                        Name = "Brand Name",
                        ImageUrl = "https://res.cloudinary.com/over-clocked/brands/image.jpg",
                    },
                    Category = new CategoryResponse
                    {
                        Id = Guid.NewGuid(),
                        Name = "Category Name",
                        ImageUrl = "https://res.cloudinary.com/over-clocked/categories/image.jpg",
                    },
                    Tags = [],
                    Images = [],
                    Specifications = [],
                    RatingsBreakdown = new Dictionary<int, int>(),
                }
            );

        _reviewReadRepositoryMock
            .GetProductRatingsBreakdownAsync(Arg.Any<ProductId>(), Arg.Any<CancellationToken>())
            .Returns(
                new Dictionary<int, int>
                {
                    { 1, 0 },
                    { 2, 0 },
                    { 3, 0 },
                    { 4, 0 },
                    { 5, 0 },
                }
            );

        // Act
        Result<ProductResponse> result = await _getProductByIdRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBe(Error.None);

        await _productReadRepositoryMock
            .Received(1)
            .GetByIdAsync(
                Arg.Any<ProductId>(),
                Arg.Any<Expression<Func<Product, ProductResponse>>>(),
                Arg.Any<CancellationToken>()
            );

        await _reviewReadRepositoryMock
            .Received(1)
            .GetProductRatingsBreakdownAsync(Arg.Any<ProductId>(), Arg.Any<CancellationToken>());
    }
}
