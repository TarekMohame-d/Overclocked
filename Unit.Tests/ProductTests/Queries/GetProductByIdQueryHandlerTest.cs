using System.Net;
using Application.Abstraction.Repositories;
using Application.Features.Brand.Queries.GetBrandById;
using Application.Features.Category.Queries.GetCategoryById;
using Application.Features.Product.Queries.GetProductById;
using Application.Features.Tag.Queries.GetTagById;
using Domain.Entities;
using NSubstitute;
using Shouldly;

namespace Unit.Tests.ProductTests.Queries;

public class GetProductByIdQueryHandlerTest
{
    private readonly IProductRepository _productRepositoryMock;
    private readonly GetProductByIdQueryHandler _handler;

    public GetProductByIdQueryHandlerTest()
    {
        _productRepositoryMock = Substitute.For<IProductRepository>();
        _handler = new GetProductByIdQueryHandler(_productRepositoryMock);
    }

    [Fact]
    public async Task Handle_WhenProductExists_ShouldReturnProduct()
    {
        // Arrange
        var productDto = new ProductDto
        {
            Name = "Product 1",
            Thumbnail = "thumbnail1.jpg",
            Description = "Description 1",
            Price = 100,
            Discount = 0,
            Rating = 4.5,
            Category = Substitute.For<CategoryDto>(),
            Brand = Substitute.For<BrandDto>(),
            Tags = Substitute.For<List<TagDto>>()
        };

        _productRepositoryMock.GetProductAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(productDto);

        var query = new GetProductByIdQuery { Id = Guid.NewGuid() };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Data.ShouldNotBeNull();

        await _productRepositoryMock.Received(1)
            .GetProductAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenProductDoesNotExist_ShouldReturnFailure()
    {
        // Arrange
        _productRepositoryMock.GetProductAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((ProductDto)null!);

        var query = new GetProductByIdQuery { Id = Guid.NewGuid() };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        result.Error.ShouldNotBeNull();
    }
}
