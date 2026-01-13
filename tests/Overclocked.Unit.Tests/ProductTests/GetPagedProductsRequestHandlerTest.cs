using System.Linq.Expressions;
using NSubstitute;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Common.Enums;
using Overclocked.Application.Features.BrandUseCases.DTOs.Responses;
using Overclocked.Application.Features.ProductUseCases.DTOs.Requests;
using Overclocked.Application.Features.ProductUseCases.DTOs.Responses;
using Overclocked.Application.Features.ProductUseCases.GetPagedProducts;
using Overclocked.Architecture.Tests.FakeData;
using Overclocked.Domain.ProductAggregate;
using Overclocked.SharedKernel;
using Shouldly;

namespace Overclocked.Unit.Tests.ProductTests;

public class GetPagedProductsRequestHandlerTest
{
    private readonly IProductReadRepository _productReadRepositoryMock;
    private readonly GetPagedProductsRequestHandler _getPagedProductsRequestHandler;

    public GetPagedProductsRequestHandlerTest()
    {
        _productReadRepositoryMock = Substitute.For<IProductReadRepository>();

        _getPagedProductsRequestHandler = new GetPagedProductsRequestHandler(_productReadRepositoryMock);
    }

    [Fact]
    public async Task GetPagedProductsRequestHandler_Should_ReturnEmptyList_When_ProductsDoesNotExist()
    {
        // Arrange
        _productReadRepositoryMock
            .CountAsync(
                Arg.Any<string>(),
                Arg.Any<Guid>(),
                Arg.Any<Guid>(),
                Arg.Any<Guid>(),
                Arg.Any<bool>(),
                Arg.Any<CancellationToken>()
            )
            .Returns(0);

        var query = new GetPagedProductsQuery(
            1,
            10,
            string.Empty,
            string.Empty,
            string.Empty,
            false,
            Guid.Empty,
            Guid.Empty,
            Guid.Empty
        );

        var request = GetPagedProductsRequest.FromRequest(query);

        // Act
        Result<PagedResult<ProductPagedResponse>> result = await _getPagedProductsRequestHandler.Handle(
            request,
            CancellationToken.None
        );

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Items.ShouldBeEmpty();
        result.Error.ShouldBe(Error.None);

        await _productReadRepositoryMock
            .Received(1)
            .CountAsync(
                Arg.Any<string>(),
                Arg.Any<Guid>(),
                Arg.Any<Guid>(),
                Arg.Any<Guid>(),
                Arg.Any<bool>(),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task GetPagedProductsRequestHandler_Should_ReturnResults_When_ProductsExists()
    {
        // Arrange
        var query = new GetPagedProductsQuery(
            1,
            10,
            string.Empty,
            string.Empty,
            string.Empty,
            false,
            Guid.Empty,
            Guid.Empty,
            Guid.Empty
        );

        var request = GetPagedProductsRequest.FromRequest(query);

        List<Product> products = new ProductFaker(Guid.NewGuid(), Guid.NewGuid()).Generate(10);

        _productReadRepositoryMock
            .CountAsync(
                Arg.Any<string>(),
                Arg.Any<Guid>(),
                Arg.Any<Guid>(),
                Arg.Any<Guid>(),
                Arg.Any<bool>(),
                Arg.Any<CancellationToken>()
            )
            .Returns(10);

        _productReadRepositoryMock
            .GetPagedAsync(
                Arg.Any<int>(),
                Arg.Any<int>(),
                Arg.Any<string>(),
                Arg.Any<Guid>(),
                Arg.Any<Guid>(),
                Arg.Any<Guid>(),
                Arg.Any<ProductSortField>(),
                Arg.Any<Application.Common.Enums.SortDirection>(),
                Arg.Any<bool>(),
                Arg.Any<Expression<Func<Product, ProductPagedResponse>>>(),
                Arg.Any<CancellationToken>()
            )
            .Returns(MapToProductPagedResponses(products));

        // Act
        Result<PagedResult<ProductPagedResponse>> result = await _getPagedProductsRequestHandler.Handle(
            request,
            CancellationToken.None
        );

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Items.ShouldNotBeEmpty();
        result.Error.ShouldBe(Error.None);

        await _productReadRepositoryMock
            .Received(1)
            .CountAsync(
                Arg.Any<string>(),
                Arg.Any<Guid>(),
                Arg.Any<Guid>(),
                Arg.Any<Guid>(),
                Arg.Any<bool>(),
                Arg.Any<CancellationToken>()
            );

        await _productReadRepositoryMock
            .Received(1)
            .GetPagedAsync(
                Arg.Any<int>(),
                Arg.Any<int>(),
                Arg.Any<string>(),
                Arg.Any<Guid>(),
                Arg.Any<Guid>(),
                Arg.Any<Guid>(),
                Arg.Any<ProductSortField>(),
                Arg.Any<Application.Common.Enums.SortDirection>(),
                Arg.Any<bool>(),
                Arg.Any<Expression<Func<Product, ProductPagedResponse>>>(),
                Arg.Any<CancellationToken>()
            );
    }

    private static List<ProductPagedResponse> MapToProductPagedResponses(List<Product> products) =>
        products.ConvertAll(product => new ProductPagedResponse
        {
            Id = product.Id.Value,
            Name = product.Name,
            Thumbnail = product.Thumbnail.Value,
            Price = product.Price.Value,
            Discount = product.Discount.Value,
            Rating = product.ProductRating.AverageRating,
            ReviewCount = product.ProductRating.ReviewCount,
            Brand = new BrandResponse
            {
                Id = product.BrandId.Value,
                Name = "Brand Name",
                ImageUrl = "https://res.cloudinary.com/over-clocked/brands/image.jpg",
            },
        });
}
