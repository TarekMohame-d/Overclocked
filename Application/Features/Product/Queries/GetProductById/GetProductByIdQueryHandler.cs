using System.Net;
using Application.Abstraction.Messaging;
using Application.Abstraction.Repositories;
using Application.Common.Results;

namespace Application.Features.Product.Queries.GetProductById;

public class GetProductByIdQueryHandler : IQueryHandler<GetProductByIdQuery, Result<ProductDto>>
{
    private readonly IProductRepository _productRepository;

    public GetProductByIdQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<Result<ProductDto>> Handle(GetProductByIdQuery query, CancellationToken cancellationToken)
    {
        var productDto = await _productRepository.GetProductAsync(query.Id, cancellationToken);

        if (productDto is null)
            return Result<ProductDto>.Failure(Errors.ProductNotFound, HttpStatusCode.NotFound);

        return Result<ProductDto>.Success(productDto);
    }
}
