using System.Net;
using Application.Abstraction.DomainServices;
using Application.Abstraction.Messaging;
using Application.Abstraction.Repositories;
using Application.Common.Results;
using Application.Services.Product;
using Application.Services.Product.DTOs.Request;
using Application.Services.Product.DTOs.Response;
using Application.Services.Review.DTOs.Response;
using ArchitectureTests.FakeData;
using Domain.Entities;
using MockQueryable;
using NSubstitute;
using Shouldly;

namespace Unit.Tests.ProductTests;

public class GetProductByIdAsyncTest
{
    private readonly IEventDispatcher _eventDispatcherMock;
    private readonly IProductRepository _productRepositoryMock;
    private readonly IReviewService _reviewServiceMock;
    private readonly ProductService _productService;
    private readonly IUnitOfWork _unitOfWorkMock;

    public GetProductByIdAsyncTest()
    {
        _productRepositoryMock = Substitute.For<IProductRepository>();
        _reviewServiceMock = Substitute.For<IReviewService>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _eventDispatcherMock = Substitute.For<IEventDispatcher>();

        _productService = new ProductService(
            _productRepositoryMock,
            _reviewServiceMock,
            _unitOfWorkMock,
            _eventDispatcherMock);
    }

    [Fact]
    public async Task GetProductByIdAsync_Should_ReturnProduct_When_ProductExists()
    {
        // Arrange
        List<Product> products = new ProductFaker().Generate(3);
        Category category = new CategoryFaker().Generate();
        Brand brand = new BrandFaker().Generate();
        var specification = new Specification()
        {
            ProductId = products[0].Id,
            Name = "Key",
            Value = "Value"
        };

        products[0].Category = category;
        products[0].Brand = brand;
        products[0].TagProducts.Add(new TagProduct()
        {
            ProductId = products[0].Id,
            TagId = Guid.NewGuid()
        });
        products[0].Specifications.Add(specification);

        IQueryable<Product> mockQueryable = products.BuildMock();

        _productRepositoryMock.Query()
            .Returns(mockQueryable);

        _reviewServiceMock.GetReviewRatingBreakdownAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(Substitute.For<Result<RatingBreakdownResponse>>());

        var request = new GetProductByIdRequest { Id = products[0].Id };

        // Act
        Result<ProductResponse> result = await _productService.GetProductByIdAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Data.ShouldNotBeNull();

        _productRepositoryMock.Received(1)
            .Query();

        await _reviewServiceMock.Received(1)
            .GetReviewRatingBreakdownAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetProductByIdAsync_Should_ReturnFailure_When_ProductDoesNotExist()
    {
        // Arrange
        List<Product> products = new ProductFaker().Generate(3);

        IQueryable<Product> mockQueryable = products.BuildMock();

        _productRepositoryMock.Query()
            .Returns(mockQueryable);

        var request = new GetProductByIdRequest { Id = Guid.NewGuid() };

        // Act
        Result<ProductResponse> result = await _productService.GetProductByIdAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Data.ShouldBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        result.Error.ShouldNotBeNull();

        _productRepositoryMock.Received(1)
            .Query();
    }
}
