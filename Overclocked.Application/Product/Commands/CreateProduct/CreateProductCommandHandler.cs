using System.Net;
using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Product.Mapping;
using Overclocked.Domain.Common.Results;
using productEntity = Overclocked.Domain.ProductAggregate.Product;

namespace Overclocked.Application.Product.Commands.CreateProduct;

public class CreateProductCommandHandler(
    IProductRepository productRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<CreateProductCommand>
{
    public async Task<Result> Handle(CreateProductCommand command, CancellationToken cancellationToken)
    {
        productEntity product = command.ToEntity();

        await productRepository.AddAsync(product, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(HttpStatusCode.Created);
    }
}
